using StatefulMenu.Core.Attributes;

namespace StatefulMenu.IntegrationTests.Models;

public class NullableAndRetryModel
{
    [InputField("Optional number", Order = 1, IsRequired = false, Pattern = "^\\d+$", ErrorMessage = "digits only")]
    public int? Number { get; set; }
}