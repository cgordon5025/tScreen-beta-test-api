using System.Collections;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Core.Extensions;

public static class ModelStateExtensions
{
    public static Hashtable ErrorsToHashtable(this ModelStateDictionary modelState)
    {
        var errors = new Hashtable();

        if (modelState.IsValid) return null;

        foreach (var (key, value) in modelState)
            if (value.Errors.Any())
                errors[key.Replace("model.", "")] =
                    value.Errors
                        .Select(err => err.ErrorMessage)
                        .FirstOrDefault();

        return errors;
    }
}