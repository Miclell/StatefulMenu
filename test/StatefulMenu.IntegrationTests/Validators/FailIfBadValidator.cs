using System;

namespace StatefulMenu.IntegrationTests.Validators;

public class FailIfBadValidator
{
    public bool Validate(string input, out string errorMessage)
    {
        if (string.Equals(input, "bad", StringComparison.OrdinalIgnoreCase))
        {
            errorMessage = "bad input";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}