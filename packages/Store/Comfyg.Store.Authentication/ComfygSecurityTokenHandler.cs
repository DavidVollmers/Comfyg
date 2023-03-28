using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Comfyg.Store.Authentication.Abstractions;
using Comfyg.Store.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Comfyg.Store.Authentication;

internal class ComfygSecurityTokenHandler : JwtSecurityTokenHandler
{
    private readonly IClientService _clientService;
    private readonly IConfiguration _configuration;

    public ComfygSecurityTokenHandler(IClientService clientService, IConfiguration configuration)
    {
        _clientService = clientService ?? throw new ArgumentNullException(nameof(clientService));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters,
        out SecurityToken validatedToken)
    {
        if (token == null) throw new ArgumentNullException(nameof(token));
        if (validationParameters == null) throw new ArgumentNullException(nameof(validationParameters));

        var jwt = ReadJwtToken(token);

        IClient? client = null;
        string clientSecret = null!;

        var systemClient = new SystemClient(_configuration);
        if (systemClient.IsConfigured && jwt.Issuer == systemClient.ClientId)
        {
            clientSecret = systemClient.ClientSecret;
            client = systemClient;
        }

        if (client == null)
        {
            client = _clientService.GetClientAsync(jwt.Issuer).GetAwaiter().GetResult();
            if (client == null) throw new SecurityTokenInvalidIssuerException("Issuer is no valid Comfyg client.");

            clientSecret = _clientService.ReceiveClientSecretAsync(client).GetAwaiter().GetResult();
        }

        if (clientSecret == null) throw new SecurityTokenInvalidSigningKeyException("Missing client secret.");

        validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(clientSecret));

        var principal = base.ValidateToken(token, validationParameters, out validatedToken);

        return new ClaimsPrincipal(new ClientIdentity(client, principal.Identity!));
    }
}
