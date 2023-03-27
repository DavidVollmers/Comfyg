using System.Reflection;
using System.Text;
using CliWrap;

namespace Comfyg.Cli.Tests;

public class TestCliResult
{
    public string Output { get; set; }

    public string Error { get; set; }

    public int ExitCode { get; set; }
}

public static class TestCli
{
    public static async Task<TestCliResult> ExecuteAsync(string arguments)
    {
        var path = Assembly.GetAssembly(typeof(TestCli))!.Location.Replace(".Tests.dll", ".exe");
        
        var stdOutBuffer = new StringBuilder();
        var stdErrorBuffer = new StringBuilder();
        var result =  await CliWrap.Cli.Wrap(path)
            .WithArguments(arguments)
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrorBuffer))
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
            .WithValidation(CommandResultValidation.None)
            .ExecuteAsync();

        return new TestCliResult
        {
            Error = stdErrorBuffer.ToString(), ExitCode = result.ExitCode, Output = stdOutBuffer.ToString()
        };
    }
}
