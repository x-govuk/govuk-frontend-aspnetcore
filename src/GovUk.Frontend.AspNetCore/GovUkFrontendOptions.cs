using GovUk.Frontend.AspNetCore.ModelBinding;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Http;

namespace GovUk.Frontend.AspNetCore;

/// <summary>
/// Options for configuring GovUk.Frontend.AspNetCore.
/// </summary>
public class GovUkFrontendOptions
{
    private readonly DateInputConverterRegistry _dateInputConverterRegistry;

    /// <summary>
    /// Creates a new <see cref="GovUkFrontendOptions"/>.
    /// </summary>
    public GovUkFrontendOptions()
    {
        AcceptMonthNamesInDateInputs = true;
        AddNovalidateAttributeToForms = true;

        _dateInputConverterRegistry = new();
        RegisterDateInputModelConverter(DateTimeDateInputModelConverter.ModelType, new DateTimeDateInputModelConverter());
        RegisterDateInputModelConverter(DateOnlyDateInputModelConverter.ModelType, new DateOnlyDateInputModelConverter());
        RegisterDateInputModelConverter(TupleDateInputModelConverter.ModelType, new TupleDateInputModelConverter());

        ErrorSummaryGeneration = ErrorSummaryGenerationOptions.PrependToMainElement;
        PrependErrorToTitle = true;
        FrontendPackageHostingOptions = FrontendPackageHostingOptions.HostAssets |
            FrontendPackageHostingOptions.HostCompiledFiles |
            FrontendPackageHostingOptions.RemoveSourceMapReferences;
    }

    /// <summary>
    /// Whether to accept full and abbreviated month names in date input components.
    /// </summary>
    /// <remarks>
    /// The default is <c>true</c>.
    /// </remarks>
    public bool AcceptMonthNamesInDateInputs { get; set; }

    /// <summary>
    /// Whether to add a <c>novalidate</c> attribute to <c>form</c> elements.
    /// </summary>
    /// <remarks>
    /// The default is <c>true</c>.
    /// </remarks>
    public bool AddNovalidateAttributeToForms { get; set; }

    /// <inheritdoc cref="AspNetCore.FrontendPackageHostingOptions"/>
    public FrontendPackageHostingOptions FrontendPackageHostingOptions { get; set; }

    /// <summary>
    /// The default value for <see cref="ButtonTagHelper.PreventDoubleClick"/>.
    /// </summary>
    public bool? DefaultButtonPreventDoubleClick { get; set; }

    /// <summary>
    /// The default value for <see cref="FileUploadTagHelper.JavaScriptEnhancements"/>.
    /// </summary>
    public bool? DefaultFileUploadJavaScriptEnhancements { get; set; }

    /// <summary>
    /// A delegate for retrieving a CSP nonce for the current request.
    /// </summary>
    /// <remarks>
    /// This is invoked when the page template utilities generate style and script import tags to add a <c>nonce</c> attribute.
    /// </remarks>
    public Func<HttpContext, string?>? GetCspNonceForRequest { get; set; }

    /// <summary>
    /// Configures automatic error summary generation.
    /// </summary>
    public ErrorSummaryGenerationOptions ErrorSummaryGeneration { get; set; }

    /// <summary>
    /// Whether to prepend &quot;Error: &quot; to the <c>&lt;title&gt;</c> element when an error summary has been created in the current view.
    /// </summary>
    /// <remarks>
    /// The default is <c>true</c>.
    /// </remarks>
    public bool PrependErrorToTitle { get; set; }

    /// <summary>
    /// Registers a <see cref="DateInputModelConverter"/> for the specified <see cref="Type"/>.
    /// </summary>
    /// <param name="modelType">The <see cref="DateInputModelConverter"/>.</param>
    /// <param name="converter">The <see cref="Type"/>.</param>
    public void RegisterDateInputModelConverter(Type modelType, DateInputModelConverter converter)
    {
        ArgumentNullException.ThrowIfNull(modelType);
        ArgumentNullException.ThrowIfNull(converter);

        _dateInputConverterRegistry.RegisterConverter(modelType, converter);
    }

    internal DateInputModelConverter? FindDateInputModelConverterForType(Type modelType)
    {
        ArgumentNullException.ThrowIfNull(modelType);

        return _dateInputConverterRegistry.FindConverter(modelType);
    }
}
