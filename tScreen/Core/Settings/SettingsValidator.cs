using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Core.Settings;

public static class SettingsValidator
{
    public static void Validate(IValidateSettings settings)
    {
        var tree = new Dictionary<string, object>();
        var errorCount = 0;

        var map = _validateSettingsTree(settings, tree, ref errorCount);

        var serializedResult = JsonSerializer.Serialize(map, new JsonSerializerOptions { WriteIndented = true });

        if (errorCount > 0)
        {
            throw new SettingValidatorFailedException(errorCount, serializedResult);
        }

    }

    private static Dictionary<string, object> _validateSettingsTree(
        object instance, Dictionary<string, object> tree, ref int errorCount)
    {
        if (instance == null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        var groupName = Utils.NormalizeName(instance.GetType().Name);

        var validationResults = new List<ValidationResult>();

        Validator.TryValidateObject(instance, new ValidationContext(instance), validationResults, validateAllProperties: true);

        // Include details about the configuration binding class which is 
        // useful for investigating problems
        var errors = new Dictionary<string, object>
        {
            ["@Meta"] = new
            {
                BindingClassName = instance.GetType().FullName,
                AssemblyName = instance.GetType().Assembly.FullName
            }
        };

        if (validationResults.Any())
        {
            foreach (var validationResult in validationResults)
            {
                var setting = validationResult.MemberNames.FirstOrDefault();
                if (string.IsNullOrEmpty(setting))
                    continue;

                errors[setting] = validationResult.ErrorMessage;
                errorCount++;
            }

            tree.Add(groupName, errors);
        }

        var propertiesWithInterface = GetPropertiesThatHaveInterface<IValidateSettings>(instance);

        var withInterface = propertiesWithInterface.ToList();
        if (!withInterface.Any())
            return tree;

        foreach (var property in withInterface)
        {
            var propertyValue = property.GetValue(instance);

            if (propertyValue == null)
            {
                throw new Exception(
                    $"Expected settings group, got NULL instead for nested property {property.Name} " +
                    $"in {groupName}. Either the configuration name doesn't match a configuration provider " +
                    $"(e.g., appsettings*.json) or minimum definition is not defined");
            }

            tree[groupName] = _validateSettingsTree(propertyValue, errors, ref errorCount);
        }

        return tree;
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static IEnumerable<PropertyInfo> GetPropertiesThatHaveInterface<TInterface>(object instance)
    {
        var instanceType = typeof(TInterface);
        return instance
            .GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.PropertyType.GetInterface(instanceType.Name) == instanceType);
    }
}
