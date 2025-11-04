# StatefulMenu

![NuGet](https://img.shields.io/nuget/v/StatefulMenu.svg)
![Tests](https://github.com/Miclell/StatefulMenu/actions/workflows/dotnet.yml/badge.svg)
[Russian README](./README.md)
Documentation: [docs/en](./docs/en/guide.md) · [docs/ru](./docs/ru/guide.md)

A minimalist library for building stack-based console menu applications in .NET with navigation, hotkeys, localization, and safe data input.

## Features
- ✅ Interactive menus and stack-based screen navigation
- ✅ Hotkeys, arrow keys, numeric selection (multi-digit), Esc for "back", zero item `0`
- ✅ Console input for models: required/optional fields, `nullable`
- ✅ Validation: RegEx and custom validators/converters
- ✅ Built-in localization (ru/en)
- ✅ DI extension and auto-registration of menu commands/providers
- ✅ Custom header (`MenuHeaderOptions`), hidden items (`MenuItem.Hidden`), `MenuItem.Back/Exit` helpers

## Installation
```bash
dotnet add package StatefulMenu
```
## Quick Start
Register services in DI and start navigation from the root menu.

```csharp
using Microsoft.Extensions.DependencyInjection;
using StatefulMenu;
using StatefulMenu.Commands.Interfaces;
using StatefulMenu.Core.Interfaces;
using StatefulMenu.Core.Models;

var services = new ServiceCollection()
    // Scans current assembly tree and registers IMenuProvider/IMenuCommand
    .AddStatefulMenu();

var provider = services.BuildServiceProvider();
var nav = provider.GetRequiredService<INavigationService>();

// Root menu provider (see example below)
var root = provider.GetRequiredService<IMenuProvider>();

await nav.RunAsync(root);
```
Implement IMenuProvider and return MenuState with MenuItem entries.

```csharp
using StatefulMenu.Commands.Interfaces;
using StatefulMenu.Core.Models;

public class HomeMenuProvider : IMenuProvider
{
    public Task<MenuState> CreateMenuAsync(CancellationToken ct = default)
    {
        var items = new[]
        {
            new MenuItem("Say hello", async _ =>
            {
                Console.WriteLine("Hello!");
                return MenuResult.None();
            }, hotkey: ConsoleKey.H),

            new MenuItem("Submenu", async _ =>
            {
                var sub = new MenuState("Submenu", new[]
                {
                    new MenuItem("Back", _ => Task.FromResult(MenuResult.Pop()))
                });
                return MenuResult.Push(sub);
            }) ,

            new MenuItem("Exit", _ => Task.FromResult(MenuResult.Exit()), hotkey: ConsoleKey.E)
        };

        return Task.FromResult(new MenuState("Main Menu", items));
    }
}
```

Navigation is controlled by `MenuResult` values:
- `MenuResult.None()` — stay on current screen
- `MenuResult.Push(state)` — open new screen
- `MenuResult.Replace(state)` — replace current screen
- `MenuResult.Pop(count)` — go back count screens
- `MenuResult.Exit()` — exit navigation loop

## Keyboard Controls
- **Enter/digit (1..N)**: select item
- **↑/↓**: navigate through items
- **Esc**: go back (`Pop`)
- **Hotkeys**: if `MenuItem` has `ConsoleKey` set, pressing it executes the action immediately
- **0**: zero item "Back/Exit" (can also be selected with arrows)

## Console Data Input
The `IConsoleInputService` allows requesting models by marking properties with the `InputField attribute.

```csharp
using StatefulMenu.Core.Attributes;
using StatefulMenu.Core.Interfaces;

public record CreateUserModel(
    [property: InputField("Email", Pattern = @"^[^@]+@[^@]+\\.[^@]+$", ErrorMessage = "Invalid email")]
    string Email,

    [property: InputField("Age", IsRequired = false)]
    int? Age,

    [property: InputField("Active")] 
    bool IsActive
);

// Somewhere in menu handler
var input = provider.GetRequiredService<IConsoleInputService>();
var model = await input.ReadModelAsync<CreateUserModel>();
```
Supports:
- **Required/optional fields** (`IsRequired`)
- **Nullable types**
- **RegEx** (`Pattern` + `ErrorMessage`)
- **Custom validators/converters** via `Validators`/`Converters` (types with `public method bool Validate(string input, out string? error)` and converters returning value/error)
- **Enum fields** (with hint of allowed values)
- **Form title** via class attribute `[InputModel("Title")]`
- 
Message localization (ru/en) is automatically selected based on `CultureInfo.CurrentCulture`.

## Common Services
- `INavigationService` — stack screen management, `Navigating/Navigated events`
- `IDataService` — simple key/value store for data exchange between screens
- `MenuRenderer` — renders current screen to console (viewport with centering, header, 0-item)

## DI and Auto-Registration
`services.AddStatefulMenu()`:
- Registers localizer, renderer, input service, navigation, and store
- Scans specified assemblies (or calling assembly by default) for implementations of IMenuProvider and IMenuCommand and registers them

## Structure Tips
- Keep each screen as IMenuProvider
- Return MenuResult from menu item actions
- For complex scenarios, store data in IDataService
- For backward compatibility: simply add BackCommand/ExitCommand/HomeCommand to command list — they're automatically rendered as "0"

## Examples
- Full demo project: temp/ClinicDemo

## Requirements
- .NET 9.0+

## License
MIT — see `LICENSE`.