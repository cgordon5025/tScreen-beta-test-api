using System;

namespace Core.Settings;

public class SettingValidatorFailedException : Exception
{
    public SettingValidatorFailedException(string message)
        : base(message)
    {
    }

    public SettingValidatorFailedException(int count, string resultObject)
        : base(WriteMessage(count, resultObject))
    {
    }

    private static string WriteMessage(int count, string resultObject)
    {
        const string singular = "setting";
        const string plural = "settings";

        return $@"Found {count} {(count > 0 ? plural : singular)} which failed validation.
Validation result object:
{resultObject}";
    }
}