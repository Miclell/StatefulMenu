using System.Reflection;
using System.Runtime.CompilerServices;
using StatefulMenu.Commands.BuiltIn;
using StatefulMenu.Commands.Interfaces;
using StatefulMenu.Core.Models;

namespace StatefulMenu.Infrastructure.Components;

internal class MenuItemUtilities
{
    public static bool IsZero(MenuItem item)
    {
        if (item.IsHidden) return false;
        if (item.IsZeroIndex) return true;
        return TryDetectBuiltInZero(item);
    }

    private static bool TryDetectBuiltInZero(MenuItem item)
    {
        var target = item.Action.Target;
        if (target is null) return false;

        var fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        foreach (var field in fields)
        {
            if (!typeof(IMenuCommand).IsAssignableFrom(field.FieldType)) continue;
            var value = field.GetValue(target) as IMenuCommand;
            if (value is null) continue;
            if (value is BackCommand or ExitCommand or HomeCommand) return true;
        }

        return false;
    }
}


