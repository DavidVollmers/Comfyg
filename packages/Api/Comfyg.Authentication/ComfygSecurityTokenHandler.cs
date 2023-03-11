using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Comfyg.Authentication.Abstractions;
using Microsoft.IdentityModel.Tokens;

namespace Comfyg.Authentication;

internal class ComfygSecurityTokenHandler : JwtSecurityTokenHandler
{
    private readonly IClientService _clientService;

    public ComfygSecurityTokenHandler(IClientService clientService)
    {
        _clientService = clientService;
    }

    public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters,
        out SecurityToken validatedToken)
    {
        var jwt = ReadJwtToken(token);

        var client = _clientService.GetClientAsync(jwt.Issuer).GetAwaiter().GetResult();
        if (client == null) throw new SecurityTokenInvalidIssuerException("Issuer is no valid Comfyg client");

        var clientSecret = _clientService.ReceiveClientSecretAsync(client).GetAwaiter().GetResult();
        validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(clientSecret));

        var principal = base.ValidateToken(token, validationParameters, out validatedToken);

        return new ClaimsPrincipal(new ClientIdentity(client, principal.Identity!));
    }
}