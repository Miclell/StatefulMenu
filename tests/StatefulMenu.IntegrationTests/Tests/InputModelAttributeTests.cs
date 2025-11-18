using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using StatefulMenu.Core.Attributes;
using StatefulMenu.Infrastructure.Localization;
using StatefulMenu.Infrastructure.Services;
using Xunit;

namespace StatefulMenu.IntegrationTests.Tests;

public class InputModelAttributeTests
{
    [InputModel("Custom Header Title")]
    private record CustomInput(
        [property: InputField("Name")] string Name
    );

    [Fact]
    public async Task UsesCustomModelTitleInHeader()
    {
        var script = "Bob" + Environment.NewLine;
        var reader = new StringReader(script);
        var writer = new StringWriter();
        var origIn = Console.In;
        var origOut = Console.Out;
        try
        {
            Console.SetIn(reader);
            Console.SetOut(writer);

            var svc = new ConsoleInputService(new EnConsoleLocalizer(), CultureInfo.GetCultureInfo("en-US"));
            var value = await svc.ReadModelAsync<CustomInput>();

            Assert.NotNull(value);
            var output = writer.ToString();
            Assert.Contains("Input for Custom Header Title", output);
        }
        finally
        {
            Console.SetIn(origIn);
            Console.SetOut(origOut);
        }
    }
}


