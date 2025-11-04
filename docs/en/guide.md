# StatefulMenu — Guide (EN)

## Overview
Minimal library for building stateful console menus with stack navigation, hotkeys, localization, and safe input.

- Stack-based navigation and providers/commands
- Hotkeys, arrows, numeric selection (multi-digit), Esc=Back, zero item (0)
- Console input with attributes, validators, converters, enums
- Localization (ru/en)
- DI extension with auto-scan of providers/commands

## Quick Start
- Register services and run navigation from a root provider.
- Implement `IMenuProvider` returning `MenuState` with `MenuItem`s.

## Providers & Commands
- Keep each screen as `IMenuProvider`.
- Commands implement `IMenuCommand` and encapsulate actions.
- Build items from commands: `new MenuItem(cmd.Title, _ => cmd.ExecuteAsync(ct))`.

## Navigation Model
`MenuResult.None/Push/Replace/Pop/Exit`

## Input Service
- `[InputField]` on properties
- `[InputModel("Custom Title")]` on class to control header title

## Zero Item (0)
- Use helpers: `MenuItem.Back()/Exit()`.
- Backwards compatible: add `BackCommand/ExitCommand/HomeCommand` to commands — renderer detects them as zero (0) automatically, no `MenuItem.Back()` required.
- Pressing `0` triggers zero item immediately; arrows can select it.

## Hidden Items
- `MenuItem.Hidden(...)` excludes an item from list and numeric selection; hotkeys still work.

## Header and Layout
- `MenuHeaderOptions { Separator, Segments }`
- If `Header` is set, title is merged into the first header cell and `=== Title ===` line is suppressed.

## Long Lists & Selection
- Renderer keeps selection centered with a viewport and shows `...` when truncated.
- Multi-digit numeric selection supported (e.g. type 12 + Enter).

## DI Scanning
- `services.AddStatefulMenu()` scans calling assembly for providers and commands.

## Sample
See `temp/ClinicDemo` for a full provider/command architecture demo.
