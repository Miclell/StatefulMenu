using System.Threading;
using StatefulMenu.Commands.BuiltIn;
using StatefulMenu.Core.Models;
using StatefulMenu.Infrastructure.Components;
using Xunit;

namespace StatefulMenu.IntegrationTests.Tests;

public class MenuItemUtilitiesTests
{
    [Fact]
    public void DetectsBackCommandAsZero()
    {
        var back = new BackCommand();
        var item = new MenuItem(back.Title, _ => back.ExecuteAsync(CancellationToken.None));
        Assert.True(MenuItemUtilities_IsZero(item));
    }

    private static bool MenuItemUtilities_IsZero(MenuItem item)
    {
        // use reflection since helper is internal
        var type = typeof(MenuItemUtilities);
        var m = type.GetMethod("IsZero", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
        return (bool)(m!.Invoke(null, new object[] { item }) ?? false);
    }
}


