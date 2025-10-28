using StatefulMenu.Core.Attributes;

namespace SimpleMenuDemo.Models;

public record UserForm(
    [property: InputField("Имя", Order = 1, IsRequired = true)] string Name,
    [property: InputField("Возраст", Order = 2, IsRequired = false)] int? Age,
    [property: InputField("Email", Order = 3, Pattern = @"^[^@]+@[^@]+\.[^@]+$", ErrorMessage = "Некорректный email")] string Email
);


