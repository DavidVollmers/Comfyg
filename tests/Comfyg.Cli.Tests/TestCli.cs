using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using CliWrap;

namespace Comfyg.Cli.Tests;

public class TestCliResult
{
    public string Output { get; set; } = null!;

    public string Error { get; set; } = null!;

    public int ExitCode { get; set; }
}

public static class TestCli
{
    public static async Task<TestCliResult> ExecuteAsync(string arguments)
    {
        var path = Assembly.GetAssembly(typeof(TestCli))!.Location.Replace(".Tests.dll", ".exe");

        var stdOutBuffer = new StringBuilder();
        var stdErrorBuffer = new StringBuilder();
        var result = await CliWrap.Cli.Wrap(path)
            .WithArguments(arguments)
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrorBuffer))
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
            .WithValidation(CommandResultValidation.None)
            .ExecuteAsync();

        // https://stackoverflow.com/a/51141872/4382610
        var regex = new Regex(@"\x1B\[[0-9;]+[A-Za-z]");

        return new TestCliResult
        {
            ExitCode = result.ExitCode,
            Error = regex.Replace(stdErrorBuffer.ToString(), string.Empty),
            Output = regex.Replace(stdOutBuffer.ToString(), string.Empty)
        };
    }
}
