using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using StatefulMenu.Infrastructure.Localization;
using StatefulMenu.Infrastructure.Services;
using StatefulMenu.IntegrationTests.Models;
using Xunit;

namespace StatefulMenu.IntegrationTests.Tests;

public class CancellationTests
{
    [Fact]
    public async Task CancellationTokenStopsInputAndReturnsNull()
    {
        var inputs = new[] {"value1", "value2"};
        var script = string.Join(Environment.NewLine, inputs) + Environment.NewLine;
        var reader = new StringReader(script);
        var writer = new StringWriter();
        Console.SetIn(reader);
        Console.SetOut(writer);

        var svc = new ConsoleInputService(new EnConsoleLocalizer(), CultureInfo.GetCultureInfo("en-US"));
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var model = await svc.ReadModelAsync<TwoFieldsModel>(cts.Token);
        Assert.Null(model);
    }
}