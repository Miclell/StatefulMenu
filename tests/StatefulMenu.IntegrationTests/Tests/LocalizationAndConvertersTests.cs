using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using StatefulMenu.Infrastructure.Localization;
using StatefulMenu.Infrastructure.Services;
using StatefulMenu.IntegrationTests.Models;
using Xunit;

namespace StatefulMenu.IntegrationTests.Tests;

public class LocalizationAndConvertersTests
{
    [Fact]
    public async Task ReadsEnglishWithConvertersAndCulture()
    {
        var inputScript = string.Join(Environment.NewLine, new[] {"Alice", "25", "yes"}) + Environment.NewLine;

        var reader = new StringReader(inputScript);
        var writer = new StringWriter();
        var origIn = Console.In;
        var origOut = Console.Out;
        try
        {
            Console.SetIn(reader);
            Console.SetOut(writer);

            var svc = new ConsoleInputService(new EnConsoleLocalizer(), CultureInfo.GetCultureInfo("en-US"));
            var model = await svc.ReadModelAsync<Person>();

            Assert.NotNull(model);
            Assert.Equal("Alice", model!.Name);
            Assert.Equal(25, model.Age);
            Assert.True(model.Active);
        }
        finally
        {
            Console.SetIn(origIn);
            Console.SetOut(origOut);
        }
    }
}