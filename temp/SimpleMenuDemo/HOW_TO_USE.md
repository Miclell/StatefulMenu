# Как правильно использовать StatefulMenu

## Проблемы, которые были исправлены

### 1. Баг с вводом цифр
**Проблема:** При вводе цифры сразу выполнялось действие, а не просто выбирался пункт.

**Исправление:** Теперь цифры только выбирают пункт, а для выполнения нужно нажать Enter.

### 2. Сложная архитектура
**Проблема:** Непонятная система с `IMenuCommand` и `IMenuProvider` была слишком сложной для простых случаев.

**Решение:** Используйте прямой подход через `IMenuProvider` с созданием `MenuItem` прямо в коде.

## Правильный способ создания меню

### Простой подход (рекомендуется)

```csharp
public class MyMenuProvider : IMenuProvider
{
    public Task<MenuState> CreateMenuAsync(CancellationToken ct = default)
    {
        var items = new[]
        {
            // Простое действие
            new MenuItem("Сказать привет", async _ =>
            {
                Console.WriteLine("Привет!");
                Console.ReadKey(true);
                return MenuResult.None(); // Остаемся на экране
            }, hotkey: ConsoleKey.H), // Хоткей H

            // Открытие подменю
            new MenuItem("Подменю", async _ =>
            {
                var submenu = new MenuState("Подменю", new[]
                {
                    new MenuItem("Действие 1", async _ =>
                    {
                        Console.WriteLine("Действие 1");
                        return MenuResult.None();
                    }),
                    new MenuItem("Назад", _ => Task.FromResult(MenuResult.Pop()))
                });
                return MenuResult.Push(submenu); // Открываем новое меню
            }),

            // Выход
            new MenuItem("Выход", _ => Task.FromResult(MenuResult.Exit()), hotkey: ConsoleKey.E)
        };

        return Task.FromResult(new MenuState("Главное меню", items));
    }
}
```

### Запуск приложения

```csharp
var services = new ServiceCollection()
    .AddStatefulMenu(typeof(MyMenuProvider).Assembly);

var provider = services.BuildServiceProvider();
var nav = provider.GetRequiredService<INavigationService>();
var root = provider.GetRequiredService<MyMenuProvider>();

await nav.RunAsync(root);
```

## Управление

- **Цифры 1-9:** выбрать пункт (НЕ выполнять)
- **Enter:** выполнить выбранное действие
- **Стрелки ↑↓:** навигация по пунктам
- **Esc:** назад (Pop)
- **Хоткеи:** если у пункта есть хоткей, нажатие сразу выполнит действие

## Типы результатов (MenuResult)

- `MenuResult.None()` - остаться на текущем экране
- `MenuResult.Push(state)` - открыть новый экран
- `MenuResult.Replace(state)` - заменить текущий экран
- `MenuResult.Pop(count)` - вернуться назад на count экранов
- `MenuResult.Exit()` - выйти из приложения

## Динамические меню

Для создания меню с динамическими данными:

```csharp
public Task<MenuState> CreateMenuAsync(CancellationToken ct = default)
{
    var animals = new[] { "Кот", "Собака", "Птица" };
    
    var items = animals
        .Select(animal => new MenuItem($"Выбрать {animal}", async _ =>
        {
            Console.WriteLine($"Вы выбрали: {animal}");
            return MenuResult.None();
        }))
        .Append(new MenuItem("Назад", _ => Task.FromResult(MenuResult.Pop())))
        .ToArray();

    return Task.FromResult(new MenuState("Животные", items));
}
```

## Ввод данных

Для ввода данных используйте `IConsoleInputService`:

```csharp
public class MyCommand : IMenuCommand
{
    private readonly IConsoleInputService _inputService;
    
    public async Task<MenuResult> ExecuteAsync(CancellationToken ct = default)
    {
        var model = await _inputService.ReadModelAsync<MyModel>();
        // Обработка модели...
        return MenuResult.None();
    }
}
```

С атрибутами на модели:

```csharp
public record MyModel(
    [property: InputField("Имя", IsRequired = true)] string Name,
    [property: InputField("Возраст", IsRequired = false)] int? Age
);
```
