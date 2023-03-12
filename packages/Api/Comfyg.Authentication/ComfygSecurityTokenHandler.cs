using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Comfyg.Authentication.Abstractions;
using Comfyg.Contracts.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Comfyg.Authentication;

internal class ComfygSecurityTokenHandler : JwtSecurityTokenHandler
{
    private readonly IClientService _clientService;
    private readonly IConfiguration _configuration;

    public ComfygSecurityTokenHandler(IClientService clientService, IConfiguration configuration)
    {
        _clientService = clientService;
        _configuration = configuration;
    }

    public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters,
        out SecurityToken validatedToken)
    {
        var jwt = ReadJwtToken(token);

        IClient? client = null;
        string clientSecret = null!;

        var systemClient = _configuration["ComfygSystemClient"];
        if (systemClient != null && jwt.Issuer == systemClient)
        {
            clientSecret = _configuration["ComfygSystemClientSecret"];
            client = new Client
            {
                ClientId = systemClient,
                FriendlyName = "Comfyg System Client"
            };
        }

        if (client == null)
        {
            client = _clientService.GetClientAsync(jwt.Issuer).GetAwaiter().GetResult();
            if (client == null) throw new SecurityTokenInvalidIssuerException("Issuer is no valid Comfyg client");

            clientSecret = _clientService.ReceiveClientSecretAsync(client).GetAwaiter().GetResult();
        }

        if (clientSecret == null) throw new SecurityTokenInvalidSigningKeyException("Missing client secret");

        validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(clientSecret));

        var principal = base.ValidateToken(token, validationParameters, out validatedToken);

        return new ClaimsPrincipal(new ClientIdentity(client, principal.Identity!));
    }
}