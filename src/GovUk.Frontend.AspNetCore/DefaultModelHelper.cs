using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore;

internal class DefaultModelHelper : IModelHelper
{
    private delegate string GetFullHtmlFieldNameDelegate(ViewContext viewContext, string expression);

    private static readonly GetFullHtmlFieldNameDelegate s_getFullHtmlFieldNameDelegate;

    static DefaultModelHelper()
    {
        s_getFullHtmlFieldNameDelegate =
            (GetFullHtmlFieldNameDelegate)typeof(IHtmlGenerator).Assembly
                .GetType("Microsoft.AspNetCore.Mvc.ViewFeatures.NameAndIdProvider", throwOnError: true)!
                .GetMethod("GetFullHtmlFieldName", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)!
                .CreateDelegate(typeof(GetFullHtmlFieldNameDelegate));
    }

    public virtual string? GetDescription(ModelExplorer modelExplorer) => modelExplorer.Metadata.Description;

    public virtual string? GetDisplayName(
        ModelExplorer modelExplorer,
        string expression)
    {
        // See https://github.com/aspnet/AspNetCore/blob/master/src/Mvc/Mvc.ViewFeatures/src/DefaultHtmlGenerator.cs#L427

        var displayName = modelExplorer.Metadata.DisplayName ?? modelExplorer.Metadata.PropertyName;

        if (displayName is not null && expression is not null)
        {
            displayName = displayName.Split('.').Last();
        }

        return displayName;
    }

    public virtual string GetFullHtmlFieldName(ViewContext viewContext, string expression)
    {
        ArgumentNullException.ThrowIfNull(viewContext);
        ArgumentNullException.ThrowIfNull(expression);

        return s_getFullHtmlFieldNameDelegate(viewContext, expression);
    }

    public virtual string? GetModelValue(ViewContext viewContext, ModelExplorer modelExplorer, string expression)
    {
        Guard.ArgumentNotNull(nameof(viewContext), viewContext);
        Guard.ArgumentNotNull(nameof(modelExplorer), modelExplorer);
        Guard.ArgumentNotNull(nameof(expression), expression);

        var fullName = GetFullHtmlFieldName(viewContext, expression);

        // See https://github.com/dotnet/aspnetcore/blob/9a3aacb56af7221bfb29d851ee6b7c883650ddf6/src/Mvc/Mvc.ViewFeatures/src/DefaultHtmlGenerator.cs#L714-L724

        viewContext.ViewData.ModelState.TryGetValue(fullName, out var entry);

        var value = string.Empty;
        if (entry is not null && entry.AttemptedValue is not null)
        {
            value = entry.AttemptedValue;
        }
        else if (modelExplorer.Model is not null)
        {
            value = modelExplorer.Model.ToString();
        }

        return value;
    }

    public virtual string? GetValidationMessage(
        ViewContext viewContext,
        ModelExplorer modelExplorer,
        string expression)
    {
        // See https://github.com/aspnet/AspNetCore/blob/master/src/Mvc/Mvc.ViewFeatures/src/DefaultHtmlGenerator.cs#L795

        var fullName = GetFullHtmlFieldName(viewContext, expression);

        if (!viewContext.ViewData.ModelState.ContainsKey(fullName))
        {
            return null;
        }

        var modelErrors = viewContext.ViewData.ModelState.TryGetValue(fullName, out var entry) ? entry.Errors : null;

        ModelError? modelError = null;
        if (modelErrors is not null && modelErrors.Count != 0)
        {
            modelError = modelErrors.FirstOrDefault(m => !string.IsNullOrEmpty(m.ErrorMessage)) ?? modelErrors[0];
        }

        if (modelError is null)
        {
            return null;
        }

        return modelError.ErrorMessage;
    }
}
