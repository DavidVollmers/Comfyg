using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Comfyg.Client;

public sealed class ComfygClient : IDisposable
{
    private readonly HttpClient _httpClient = new();
    private readonly string _clientId;
    private readonly string _clientSecret;

    public ComfygClient(string connectionString)
    {
        if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));

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

    public async Task EstablishConnectionAsync(CancellationToken cancellationToken = default)
    {
    }

    private string CreateToken()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_clientSecret));

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, _clientId)
            }),
            //TODO adjustable
            Expires = DateTime.UtcNow.AddDays(1),
            Issuer = _clientId,
            Audience = _clientId,
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}