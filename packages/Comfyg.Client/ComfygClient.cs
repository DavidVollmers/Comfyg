using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Comfyg.Contracts.Authentication;
using Comfyg.Contracts.Responses;
using Microsoft.IdentityModel.Tokens;

namespace Comfyg.Client;

public sealed class ComfygClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _clientId;
    private readonly string _clientSecret;

    private SecurityToken? _token;

    public ComfygClient(string connectionString) : this(connectionString, new HttpClient())
    {
    }

    internal ComfygClient(string connectionString, HttpClient httpClient)
    {
        if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));
        _httpClient = httpClient;

        try
        {
            var connectionInformation =
                connectionString.Split(';').Select(i => i.Split('=')).ToDictionary(i => i[0], i => i[1]);

            if (!connectionInformation.ContainsKey("Endpoint")) throw new Exception("Missing \"Endpoint\" information");
            _httpClient.BaseAddress = new Uri(connectionInformation["Endpoint"]);

            if (!connectionInformation.ContainsKey("ClientId")) throw new Exception("Missing \"ClientId\" information");
            _clientId = connectionInformation["ClientId"];

            if (!connectionInformation.ContainsKey("ClientSecret"))
                throw new Exception("Missing \"ClientSecret\" information");
            _clientSecret = connectionInformation["ClientSecret"];
        }
        catch (Exception exception)
        {
            throw new ArgumentException("Invalid connection string", nameof(connectionString), exception);
        }
    }

    public async Task<ConnectionResponse> EstablishConnectionAsync(CancellationToken cancellationToken = default)
    {
        var token = CreateToken();

        var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, "connections/establish")
        {
            Headers =
            {
                { "Authorization", $"Bearer {token}" }
            }
        }, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to establish connection", null,
                response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<ConnectionResponse>(cancellationToken: cancellationToken)
            .ConfigureAwait(false))!;
    }

    private string CreateToken()
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        if (_token != null && _token.ValidTo > DateTime.UtcNow.AddMinutes(5))
        {
            return tokenHandler.WriteToken(_token);
        }

        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_clientSecret));

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