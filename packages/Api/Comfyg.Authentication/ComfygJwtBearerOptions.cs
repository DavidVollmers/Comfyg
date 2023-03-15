using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Comfyg.Authentication;

internal class ComfygJwtBearerOptions : IPostConfigureOptions<JwtBearerOptions>
{
    private readonly ComfygSecurityTokenHandler _tokenHandler;

    public ComfygJwtBearerOptions(ComfygSecurityTokenHandler tokenHandler)
    {
        _tokenHandler = tokenHandler ?? throw new ArgumentNullException(nameof(tokenHandler));
    }
    
    public void PostConfigure(string name, JwtBearerOptions options)
    {
        options.SecurityTokenValidators.Clear();
        options.SecurityTokenValidators.Add(_tokenHandler);
    }
}