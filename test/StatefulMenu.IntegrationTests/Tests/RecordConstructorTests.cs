using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using StatefulMenu.Infrastructure.Localization;
using StatefulMenu.Infrastructure.Services;
using StatefulMenu.IntegrationTests.Models;
using Xunit;

namespace StatefulMenu.IntegrationTests.Tests;

public class RecordConstructorTests
{
    [Fact]
    public async Task ReadsRecordViaConstructorRu()
    {
        var inputScript = string.Join(Environment.NewLine, new[] {"31.12.2025", "10:30", "Иванов"}) +
                          Environment.NewLine;

        var reader = new StringReader(inputScript);
        var writer = new StringWriter();
        var origIn = Console.In;
        var origOut = Console.Out;
        try
        {
            Console.SetIn(reader);
            Console.SetOut(writer);

            var svc = new ConsoleInputService(new RuConsoleLocalizer(), CultureInfo.GetCultureInfo("ru-RU"));
            var booking = await svc.ReadModelAsync<Booking>();

            Assert.NotNull(booking);
            Assert.Equal(new DateOnly(2025, 12, 31), booking!.Date);
            Assert.Equal(new TimeOnly(10, 30), booking.Time);
            Assert.Equal("Иванов", booking.Doctor);
        }
        finally
        {
            Console.SetIn(origIn);
            Console.SetOut(origOut);
        }
    }
}