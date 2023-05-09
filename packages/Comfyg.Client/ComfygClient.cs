using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using Comfyg.Client.Operations;
using Comfyg.Store.Contracts;
using Comfyg.Store.Contracts.Responses;
using Microsoft.IdentityModel.Tokens;

namespace Comfyg.Client;

/// <summary>
/// The client implementation used to connect and manage a Comfyg store.
/// </summary>
public sealed partial class ComfygClient : IDisposable
{
    private const string IvDelimiter = ".";
    private const string EnvironmentVariablePrefix = "$";
    private const string E2EeNotSupportedExceptionMessage = "End-to-end encryption is only supported for asymmetric clients.";

    private readonly HttpClient _httpClient;
    private readonly string _clientId;
    private readonly byte[] _clientSecret;
    private readonly SecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();
    private readonly bool _isAsymmetric;

    private SecurityToken? _token;

    internal bool IsEncryptionEnabled { get; }

    /// <summary>
    /// The endpoint URI of the connected Comfyg store.
    /// </summary>
    public Uri EndpointUrl => _httpClient.BaseAddress!;

    /// <summary>
    /// Provides methods to manage configuration values in the connected Comfyg store.
    /// </summary>
    public IComfygValueOperations<IConfigurationValue> Configuration { get; }

    /// <summary>
    /// Provides methods to manage setting values in the connected Comfyg store.
    /// </summary>
    public IComfygValueOperations<ISettingValue> Settings { get; }

    /// <summary>
    /// Provides methods to manage secret values in the connected Comfyg store.
    /// </summary>
    public IComfygValueOperations<ISecretValue> Secrets { get; }

    /// <summary>
    /// Creates a new client which can be used to connect to the Comfyg store via the provided connection string.
    /// </summary>
    /// <param name="connectionString">The connection string used to connect to the Comfyg store.</param>
    /// <exception cref="ArgumentNullException"><paramref name="connectionString"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="connectionString"/> is not a valid connection string.</exception>
    public ComfygClient(string connectionString) : this(connectionString, new HttpClient())
    {
    }

    internal ComfygClient(string connectionString, HttpClient httpClient)
    {
        if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));

        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        try
        {
            var connectionInformation =
                connectionString
                    .Split(';')
                    .Select(i => i.Split('='))
                    .Where(i => i.Length >= 2)
                    .ToDictionary(i => i[0], i => string.Join("=", i.Skip(1)));

            if (!connectionInformation.ContainsKey("Endpoint"))
                throw new Exception("Missing \"Endpoint\" information.");
            _httpClient.BaseAddress = new Uri(GetConnectionStringValue(connectionInformation["Endpoint"]));

            if (!connectionInformation.ContainsKey("ClientId"))
                throw new Exception("Missing \"ClientId\" information.");
            _clientId = GetConnectionStringValue(connectionInformation["ClientId"]);

            if (!connectionInformation.ContainsKey("ClientSecret"))
                throw new Exception("Missing \"ClientSecret\" information.");
            var clientSecret = GetConnectionStringValue(connectionInformation["ClientSecret"]);

            if (File.Exists(clientSecret))
            {
                using var rsa = RSA.Create();
                rsa.ImportFromPem(File.ReadAllText(clientSecret));
                _clientSecret = rsa.ExportRSAPrivateKey();

                _isAsymmetric = true;
            }
            else
            {
                _clientSecret = Convert.FromBase64String(clientSecret);
                if (_clientSecret.Length < 16)
                    throw new InvalidOperationException("Client secret must be at least 16 bytes long.");
            }

            if (connectionInformation.TryGetValue("Encryption", out var encryption))
            {
                encryption = GetConnectionStringValue(encryption);
                
                IsEncryptionEnabled = bool.Parse(encryption);

                if (IsEncryptionEnabled && !_isAsymmetric)
                    throw new InvalidOperationException(E2EeNotSupportedExceptionMessage);
            }
        }
        catch (Exception exception)
        {
            throw new ArgumentException("Invalid connection string.", nameof(connectionString), exception);
        }

        Configuration = new ConfigurationValuesOperations(this);
        Settings = new SettingValuesOperations(this);
        Secrets = new SecretValuesOperations(this);
    }

    /// <summary>
    /// Establishes a connection to the Comfyg store.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> controlling the request lifespan.</param>
    /// <returns><see cref="IConnectionResponse"/></returns>
    /// <exception cref="HttpRequestException">Invalid status code is returned.</exception>
    public async Task<IConnectionResponse> EstablishConnectionAsync(CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync(() => new HttpRequestMessage(HttpMethod.Post, "connections/establish"),
            5, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to establish connection.", null,
                response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<IConnectionResponse>(cancellationToken: cancellationToken)
            .ConfigureAwait(false))!;
    }

    internal async Task<HttpResponseMessage> SendRequestAsync(Func<HttpRequestMessage> requestProvider,
        int maxTries = 1, CancellationToken cancellationToken = default)
    {
        var token = CreateToken();

        var tries = 0;
        do
        {
            var request = requestProvider();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                return await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (HttpRequestException)
            {
                if (++tries >= maxTries) throw;

                await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
            }
        } while (true);
    }

    private string CreateToken()
    {
        if (_token != null && _token.ValidTo > DateTimeOffset.UtcNow.AddMinutes(5))
        {
            return _tokenHandler.WriteToken(_token);
        }

        var securityKey = CreateSecurityKey();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] {new Claim(ClaimTypes.NameIdentifier, _clientId)}),
            //TODO adjustable
            Expires = DateTime.UtcNow.AddDays(1).AddMinutes(5),
            Issuer = _clientId,
            Audience = _clientId,
            SigningCredentials = new SigningCredentials(securityKey,
                !_isAsymmetric ? SecurityAlgorithms.HmacSha512Signature : SecurityAlgorithms.RsaSha512Signature)
        };

        _token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(_token);
    }

    private SecurityKey CreateSecurityKey()
    {
        if (!_isAsymmetric) return new SymmetricSecurityKey(_clientSecret);

        var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(_clientSecret, out _);
        return new RsaSecurityKey(rsa);
    }

    /// <summary>
    /// Disposes the client implementation and all allocated resources.
    /// </summary>
    public void Dispose()
    {
        _httpClient.Dispose();
    }

    private static string GetConnectionStringValue(string value)
    {
        if (!value.StartsWith(EnvironmentVariablePrefix)) return value;
        
        var environmentVariable = value[EnvironmentVariablePrefix.Length..];
        value = Environment.GetEnvironmentVariable(environmentVariable)!;
        if (value == null)
            throw new Exception("Environment variable does not exist: " + environmentVariable);

        return value;
    }
}
