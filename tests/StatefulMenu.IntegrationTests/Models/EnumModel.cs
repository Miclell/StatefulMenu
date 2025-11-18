using StatefulMenu.Core.Attributes;

namespace StatefulMenu.IntegrationTests.Models;

public enum Color
{
    Red,
    Green
}

public class EnumModel
{
    [InputField("Color", Order = 1)] public Color Color { get; set; }
}