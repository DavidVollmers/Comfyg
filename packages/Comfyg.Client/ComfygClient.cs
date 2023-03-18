using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Comfyg.Client.Operations;
using Comfyg.Contracts.Configuration;
using Comfyg.Contracts.Responses;
using Comfyg.Contracts.Secrets;
using Comfyg.Contracts.Settings;
using Microsoft.IdentityModel.Tokens;

namespace Comfyg.Client;

public sealed partial class ComfygClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _clientId;
    private readonly string _clientSecret;

    private SecurityToken? _token;

    public Uri EndpointUrl => _httpClient.BaseAddress!;

    public IComfygValuesOperations<IConfigurationValue> Configuration { get; }

    public IComfygValuesOperations<ISettingValue> Settings { get; }

    public IComfygValuesOperations<ISecretValue> Secrets { get; }

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
            _httpClient.BaseAddress = new Uri(connectionInformation["Endpoint"]);

            if (!connectionInformation.ContainsKey("ClientId"))
                throw new Exception("Missing \"ClientId\" information.");
            _clientId = connectionInformation["ClientId"];

            if (!connectionInformation.ContainsKey("ClientSecret"))
                throw new Exception("Missing \"ClientSecret\" information.");
            _clientSecret = connectionInformation["ClientSecret"];
        }
        catch (Exception exception)
        {
            throw new ArgumentException("Invalid connection string.", nameof(connectionString), exception);
        }

        Configuration = new ConfigurationValuesOperations(this);
        Settings = new SettingValuesOperations(this);
        Secrets = new SecretValuesOperations(this);
    }

    public async Task<ConnectionResponse> EstablishConnectionAsync(CancellationToken cancellationToken = default)
    {
        var token = CreateToken();

        var request = new HttpRequestMessage(HttpMethod.Post, "connections/establish");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to establish connection.", null,
                response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<ConnectionResponse>(cancellationToken: cancellationToken)
            .ConfigureAwait(false))!;
    }

    internal async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = CreateToken();
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        return await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    private string CreateToken()
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        if (_token != null && _token.ValidTo > DateTime.UtcNow.AddMinutes(5))
        {
            return tokenHandler.WriteToken(_token);
        }

        var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(_clientSecret));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, _clientId)
            }),
            //TODO adjustable
            Expires = DateTime.UtcNow.AddDays(1).AddMinutes(5),
            Issuer = _clientId,
            Audience = _clientId,
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature)
        };

        _token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(_token);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}