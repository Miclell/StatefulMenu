using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using StatefulMenu.Core.Attributes;
using StatefulMenu.Infrastructure.Localization;
using StatefulMenu.Infrastructure.Services;
using StatefulMenu.IntegrationTests.Validators;
using Xunit;

namespace StatefulMenu.IntegrationTests.Tests;

public class ValidationTests
{
    [Fact]
    public async Task RegexAndCustomValidator_SuccessAndFailure()
    {
        var inputs = new[] {"12", "bad", "123"};
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
            var model = await svc.ReadModelAsync<RegexAndValidatorModel>();

            var output = writer.ToString();
            Assert.Contains("3 digits", output);
            Assert.Contains("bad input", output);
            Assert.NotNull(model);
            Assert.Equal("123", model!.Code);
        }
        finally
        {
            Console.SetIn(origIn);
            Console.SetOut(origOut);
        }
    }

    [Fact]
    public async Task NullableField_AllowsEmptyAndRetryOnInvalid()
    {
        var inputs = new[] {"abc", ""};
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
            var model = await svc.ReadModelAsync<NullableAndRetryModel>();

            var output = writer.ToString();
            Assert.Contains("digits only", output);
            Assert.NotNull(model);
            Assert.Null(model!.Number);
        }
        finally
        {
            Console.SetIn(origIn);
            Console.SetOut(origOut);
        }
    }

    private class RegexAndValidatorModel
    {
        [StatefulMenu.Core.Attributes.InputField("Code", Order = 1, Pattern = "^\\d{3}$", ErrorMessage = "3 digits", Validators =
            [typeof(FailIfBadValidator)])]
        public string Code { get; set; } = string.Empty;
    }

    private class NullableAndRetryModel
    {
        [StatefulMenu.Core.Attributes.InputField("Optional number", Order = 1, IsRequired = false, Pattern = "^\\d+$", ErrorMessage = "digits only")]
        public int? Number { get; set; }
    }
}