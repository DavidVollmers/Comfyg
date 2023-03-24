using Microsoft.AspNetCore.Mvc;

namespace Comfyg.Examples.AspNetCore.Controllers;

[ApiController]
public class ExampleController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public ExampleController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("example")]
    public IActionResult GetComfygValues()
    {
        return Ok(new
        {
            ConfigKey = _configuration["ConfigKey"],
            SettingKey = _configuration["SettingKey"],
            SecretKey = _configuration["SecretKey"]
        });
    }
}
