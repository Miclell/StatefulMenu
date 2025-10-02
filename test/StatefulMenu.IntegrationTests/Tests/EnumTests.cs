using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using StatefulMenu.Infrastructure.Localization;
using StatefulMenu.Infrastructure.Services;
using StatefulMenu.IntegrationTests.Models;
using Xunit;

namespace StatefulMenu.IntegrationTests.Tests;

public class EnumTests
{
    [Fact]
    public async Task EnumShowsAllowedValuesOnErrorThenSucceeds()
    {
        var inputs = new[] {"Blue", "Red"};
        var script = string.Join(Environment.NewLine, inputs) + Environment.NewLine;
        var reader = new StringReader(script);
        var writer = new StringWriter();
        var origIn = Console.In;
        var origOut = Console.Out;
        try
        {
            Console.SetIn(reader);
            Console.SetOut(writer);

            var svc = new ConsoleInputService(new EnConsoleLocalizer(), CultureInfo.GetCultureInfo("en-US"));
            var model = await svc.ReadModelAsync<EnumModel>();

            var output = writer.ToString();
            Assert.Contains("Allowed: Red, Green", output);
            Assert.NotNull(model);
            Assert.Equal(Color.Red, model!.Color);
        }
        finally
        {
            Console.SetIn(origIn);
            Console.SetOut(origOut);
        }
    }
}