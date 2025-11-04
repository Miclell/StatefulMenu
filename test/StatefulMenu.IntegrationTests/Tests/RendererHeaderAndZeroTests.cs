using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using StatefulMenu.Commands.BuiltIn;
using StatefulMenu.Core.Models;
using StatefulMenu.Infrastructure.Components;
using Xunit;

namespace StatefulMenu.IntegrationTests.Tests;

public class RendererHeaderAndZeroTests
{
    [Fact]
    public void RendersHeaderWithTitleAndSuppressesTripleEquals()
    {
        var header = new MenuHeaderOptions
        {
            Separator = " || ",
            Segments = new List<Func<string>> { () => DateTime.UtcNow.ToString("HH:mm") }
        };

        var items = new[] {new MenuItem("X", _ => System.Threading.Tasks.Task.FromResult<MenuResult>(MenuResult.None()))};
        var state = new MenuState("Main", items, header: header);

        var writer = new StringWriter();
        var origOut = Console.Out;
        try
        {
            Console.SetOut(writer);
            new MenuRenderer().Render(state, 0);
        }
        finally
        {
            Console.SetOut(origOut);
        }

        var output = writer.ToString();
        Assert.Contains("Main ||", output);
        Assert.DoesNotContain("=== Main ===", output);
    }

    [Fact]
    public void RendersImplicitZeroItemFromBackCommand()
    {
        var back = new BackCommand();
        var backItem = new MenuItem(back.Title, _ => back.ExecuteAsync(CancellationToken.None));
        var reg = new MenuItem("A", _ => System.Threading.Tasks.Task.FromResult<MenuResult>(MenuResult.None()));
        var state = new MenuState("Test", new[] {reg, backItem});

        var writer = new StringWriter();
        var origOut = Console.Out;
        try
        {
            Console.SetOut(writer);
            new MenuRenderer().Render(state, 0);
        }
        finally
        {
            Console.SetOut(origOut);
        }

        var output = writer.ToString();
        Assert.Contains("0. Назад", output);
        Assert.Contains("1. A", output);
    }

    [Fact]
    public void HiddenItemsAreNotRendered()
    {
        var hidden = MenuItem.Hidden("Hidden", _ => System.Threading.Tasks.Task.FromResult<MenuResult>(MenuResult.None()));
        var state = new MenuState("Test", new[] {hidden});

        var writer = new StringWriter();
        var origOut = Console.Out;
        try
        {
            Console.SetOut(writer);
            new MenuRenderer().Render(state, 0);
        }
        finally
        {
            Console.SetOut(origOut);
        }

        var output = writer.ToString();
        Assert.DoesNotContain("Hidden", output);
    }
}


