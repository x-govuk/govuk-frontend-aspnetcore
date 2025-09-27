using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.ConformanceTests;

public static class AttributeHelper
{
    public static AttributeDictionary MergeAttribute(
        this AttributeDictionary attributes,
        string key,
        object value)
    {
        if (value is null)
        {
            return attributes;
        }

        var newValue = AttributeValueToString(value);
        string mergedValue = attributes.ContainsKey(key)
            ? key is "class" or "aria-describedby"
                ? attributes[key] + " " + newValue
                : throw new InvalidOperationException($"Don't know how to merge attributes with key '{key}'.")
            : newValue;
        attributes[key] = mergedValue;

        return attributes;
    }

    public static AttributeDictionary ToAttributesDictionary(
        this IDictionary<string, object> attributes)
    {
        var attributeDictionary = new AttributeDictionary();

        if (attributes is not null)
        {
            foreach (var kvp in attributes)
            {
                attributeDictionary.Add(kvp.Key, AttributeValueToString(kvp.Value));
            }
        }

        return attributeDictionary;
    }

    private static string AttributeValueToString(object value) => value switch
    {
        bool b => b ? "true" : "false",
        _ => value.ToString()
    };
}
