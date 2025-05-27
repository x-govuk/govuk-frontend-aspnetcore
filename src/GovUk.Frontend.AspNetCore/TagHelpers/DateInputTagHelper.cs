using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.HtmlGeneration;
using GovUk.Frontend.AspNetCore.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using AttributeCollection = GovUk.Frontend.AspNetCore.ComponentGeneration.AttributeCollection;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS date input component.
/// </summary>
[HtmlTargetElement(TagName)]
[RestrictChildren(
    DateInputFieldsetTagHelper.TagName,
    HintTagName,
    ErrorMessageTagName,
    DateInputDayTagHelper.TagName,
    DateInputMonthTagHelper.TagName,
    DateInputYearTagHelper.TagName)]
[OutputElementHint(ComponentGenerator.FormGroupElement)]
public class DateInputTagHelper : TagHelper
{
    internal const string ErrorMessageTagName = "govuk-date-input-error-message";
    internal const string HintTagName = "govuk-date-input-hint";
    internal const string TagName = "govuk-date-input";

    private const string AspForAttributeName = "asp-for";
    private const string DateInputAttributesPrefix = "date-input-";
    private const string DisabledAttributeName = "disabled";
    private const string ForAttributeName = "for";
    private const string IdAttributeName = "id";
    private const string IgnoreModelStateErrorsAttributeName = "ignore-modelstate-errors";
    private const string NamePrefixAttributeName = "name-prefix";
    private const string ValueAttributeName = "value";

    private readonly IComponentGenerator _componentGenerator;
    private readonly IOptions<GovUkFrontendAspNetCoreOptions> _optionsAccessor;
    private readonly DateInputParseErrorsProvider _dateInputParseErrorsProvider;
    private readonly IModelHelper _modelHelper;
    private readonly HtmlEncoder _encoder;

    private object? _value;
    private bool _valueSpecified = false;

    /// <summary>
    /// Creates a <see cref="DateInputTagHelper"/>.
    /// </summary>
    public DateInputTagHelper(
        IComponentGenerator componentGenerator,
        IOptions<GovUkFrontendAspNetCoreOptions> optionsAccessor,
        DateInputParseErrorsProvider dateInputParseErrorsProvider,
        HtmlEncoder encoder)
        : this(componentGenerator, optionsAccessor, dateInputParseErrorsProvider, encoder, modelHelper: new DefaultModelHelper())
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);
        ArgumentNullException.ThrowIfNull(optionsAccessor);
        ArgumentNullException.ThrowIfNull(dateInputParseErrorsProvider);
        ArgumentNullException.ThrowIfNull(encoder);
    }

    internal DateInputTagHelper(
        IComponentGenerator componentGenerator,
        IOptions<GovUkFrontendAspNetCoreOptions> optionsAccessor,
        DateInputParseErrorsProvider dateInputParseErrorsProvider,
        HtmlEncoder encoder,
        IModelHelper modelHelper)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);
        ArgumentNullException.ThrowIfNull(optionsAccessor);
        ArgumentNullException.ThrowIfNull(dateInputParseErrorsProvider);
        ArgumentNullException.ThrowIfNull(encoder);
        ArgumentNullException.ThrowIfNull(modelHelper);
        _componentGenerator = componentGenerator;
        _optionsAccessor = optionsAccessor;
        _dateInputParseErrorsProvider = dateInputParseErrorsProvider;
        _encoder = encoder;
        _modelHelper = modelHelper;
    }

    /// <summary>
    /// An expression to be evaluated against the current model.
    /// </summary>
    [HtmlAttributeName(AspForAttributeName)]
    [Obsolete("Use the 'for' attribute instead.", DiagnosticId = DiagnosticIds.UseForAttributeInstead)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ModelExpression? AspFor
    {
        get => For;
        set => For = value;
    }

    /// <summary>
    /// An expression to be evaluated against the current model.
    /// </summary>
    [HtmlAttributeName(ForAttributeName)]
    public ModelExpression? For { get; set; }

    /// <summary>
    /// Additional attributes for the container element that wraps the items.
    /// </summary>
    [HtmlAttributeName(DictionaryAttributePrefix = DateInputAttributesPrefix)]
    public IDictionary<string, string?>? DateInputAttributes { get; set; } = new Dictionary<string, string?>();

    /// <summary>
    /// Whether the <c>disabled</c> attribute should be added to the generated <c>input</c> elements.
    /// </summary>
    [HtmlAttributeName(DisabledAttributeName)]
    public bool? Disabled { get; set; }

    /// <summary>
    /// The <c>id</c> attribute for the main component.
    /// </summary>
    /// <remarks>
    /// Also used to generate an <c>id</c> for each item's <c>input</c> when
    /// the corresponding <see cref="DateInputItemTagHelperBase.Id"/> is not specified.
    /// </remarks>
    [HtmlAttributeName(IdAttributeName)]
    public string? Id { get; set; }

    /// <summary>
    /// Whether the <see cref="ModelStateEntry.Errors"/> for the <see cref="AspFor"/> expression should be used
    /// to deduce an error message.
    /// </summary>
    /// <remarks>
    /// <para>When there are multiple errors in the <see cref="ModelErrorCollection"/> the first is used.</para>
    /// </remarks>
    [HtmlAttributeName(IgnoreModelStateErrorsAttributeName)]
    public bool? IgnoreModelStateErrors { get; set; }

    /// <summary>
    /// Optional prefix for the <c>name</c> attribute on each item's <c>input</c>.
    /// </summary>
    [HtmlAttributeName(NamePrefixAttributeName)]
    public string? NamePrefix { get; set; }

    /// <summary>
    /// The date to populate the item values with.
    /// </summary>
    [HtmlAttributeName(ValueAttributeName)]
    public object? Value
    {
        get => _value;
        set
        {
            _value = value;
            _valueSpecified = true;
        }
    }

    /// <summary>
    /// Gets the <see cref="ViewContext"/> of the executing view.
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    [DisallowNull]
    public ViewContext? ViewContext { get; set; }

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        var dateInputContext = new DateInputContext(_valueSpecified, For);
        context.SetContextItem(dateInputContext);
        context.SetContextItem<FormGroupContext3>(dateInputContext);
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var dateInputContext = context.GetContextItem<DateInputContext>();

        await output.GetChildContentAsync();

        var id = ResolveId();
        var namePrefix = ResolveNamePrefix();
        var value = ResolveValue();
        var hintOptions = dateInputContext.GetHintOptions(For, _modelHelper);
        var errorMessageOptions = dateInputContext.GetErrorMessageOptions(For, ViewContext!, _modelHelper, IgnoreModelStateErrors);

        var formGroupAttributes = new AttributeCollection(output.Attributes);
        formGroupAttributes.Remove("class", out var formGroupClasses);
        var formGroupOptions = new DateInputFormGroupOptions()
        {
            Attributes = formGroupAttributes,
            Classes = formGroupClasses
        };

        var fieldsetOptions = dateInputContext.Fieldset?.GetFieldsetOptions(_modelHelper);

        var errorItems = GetFieldsWithErrors(dateInputContext);

        var day = CreateDateInputItem(
            getComponentFromValue: date => date?.Day.ToString(),
            defaultLabel: ComponentGenerator.DateInputDefaultDayLabel,
            defaultName: DateInputModelConverterModelBinder.DayInputName,
            defaultClass: "govuk-input--width-2",
            DateInputItems.Day,
            contextItem: dateInputContext.Items.GetValueOrDefault(DateInputItemType.Day));

        var month = CreateDateInputItem(
            getComponentFromValue: date => date?.Month.ToString(),
            defaultLabel: ComponentGenerator.DateInputDefaultMonthLabel,
            defaultName: DateInputModelConverterModelBinder.MonthInputName,
            defaultClass: "govuk-input--width-2",
            DateInputItems.Month,
            contextItem: dateInputContext.Items.GetValueOrDefault(DateInputItemType.Month));

        var year = CreateDateInputItem(
            getComponentFromValue: date => date?.Year.ToString(),
            defaultLabel: ComponentGenerator.DateInputDefaultYearLabel,
            defaultName: DateInputModelConverterModelBinder.YearInputName,
            defaultClass: "govuk-input--width-4",
            DateInputItems.Year,
            contextItem: dateInputContext.Items.GetValueOrDefault(DateInputItemType.Year));

        IReadOnlyCollection<DateInputOptionsItem> items = [day, month, year];

        var attributes = new AttributeCollection(DateInputAttributes);
        attributes.Remove("class", out var classes);

        var component = await _componentGenerator.GenerateDateInputAsync(new DateInputOptions()
        {
            Id = id,
            // We've used any specified NamePrefix when creating the name for each item;
            // NamePrefix passed to generator needs to be empty to ensure names are compatible with the model binder.
            NamePrefix = null,
            Items = items,
            Hint = hintOptions,
            ErrorMessage = errorMessageOptions,
            FormGroup = formGroupOptions,
            Fieldset = fieldsetOptions,
            Classes = classes,
            Attributes = attributes
        });

        output.ApplyComponentHtml(component, HtmlEncoder.Default);

        if (errorMessageOptions is not null)
        {
            Debug.Assert(errorMessageOptions.Html is not null);

            var firstFieldWithError = items
                .First(i => i.Classes?.ToHtmlString(_encoder).Contains("govuk-input--error") == true)
                .Id;

            var containerErrorContext = ViewContext!.HttpContext.GetContainerErrorContext();
            containerErrorContext.AddError(errorMessageOptions.Html, href: "#" + firstFieldWithError);
        }

        DateInputOptionsItem CreateDateInputItem(
            Func<DateOnly?, string?> getComponentFromValue,
            string defaultLabel,
            string defaultName,
            string defaultClass,
            DateInputItems errorSource,
            DateInputContextItem? contextItem)
        {
            var haveError = errorMessageOptions is not null;

            var defaultFullName = ModelNames.CreatePropertyModelName(namePrefix, defaultName);
            var itemName = contextItem?.Name ?? defaultFullName;
            var itemId = contextItem?.Id ?? TagBuilder.CreateSanitizedId($"{id}-{contextItem?.Name ?? defaultName}", Constants.IdAttributeDotReplacement);
            var itemLabel = contextItem?.LabelHtml ?? defaultLabel;

            // Value resolution hierarchy:
            //   if Value has been set on a child tag helper e.g. <date-input-day /> then use that;
            //   if Value property is specified, use that;
            //   if AspFor is specified use value from ModelState;
            //   otherwise empty.

            var itemValue = contextItem?.ValueSpecified == true ? contextItem.Value?.ToString() ?? string.Empty :
                _valueSpecified ? getComponentFromValue(value) :
                For is not null ? GetValueFromModelState() :
                null;

            var resolvedAttributes = contextItem?.Attributes?.Clone() ?? new AttributeCollection();
            resolvedAttributes.Remove("class", out var itemClasses);
            itemClasses = itemClasses.AppendCssClasses(_encoder, defaultClass);

            if (haveError && (errorItems & errorSource) != 0)
            {
                itemClasses = itemClasses.AppendCssClasses(_encoder, "govuk-input--error");
            }

            if (Disabled == true)
            {
                resolvedAttributes.AddBoolean("disabled");
            }

            return new DateInputOptionsItem
            {
                Id = itemId,
                Name = itemName,
                Label = itemLabel,
                Value = itemValue,
                AutoComplete = contextItem?.AutoComplete,
                InputMode = contextItem?.InputMode,
                Pattern = contextItem?.Pattern,
                Classes = itemClasses,
                Attributes = resolvedAttributes
            };

            string? GetValueFromModelState()
            {
                var modelStateKey = _modelHelper.GetFullHtmlFieldName(ViewContext!, itemName.ToString());

                if (ViewContext!.ModelState.TryGetValue(modelStateKey, out var modelStateEntry) &&
                    modelStateEntry.AttemptedValue is not null)
                {
                    return modelStateEntry.AttemptedValue;
                }

                return getComponentFromValue(value);
            }
        }
    }

    private string ResolveId()
    {
        if (For is null && Id is null)
        {
            throw ExceptionHelper.AtLeastOneOfAttributesMustBeProvided(ForAttributeName, IdAttributeName);
        }

        return Id ??
            TagBuilder.CreateSanitizedId(
                _modelHelper.GetFullHtmlFieldName(ViewContext!, For!.Name),
                Constants.IdAttributeDotReplacement);
    }

    private string ResolveNamePrefix()
    {
        var resolvedName = For is not null ? _modelHelper.GetFullHtmlFieldName(ViewContext!, For.Name) : null;
        return NamePrefix ?? (resolvedName ?? string.Empty);
    }

    private DateOnly? ResolveValue()
    {
        return _valueSpecified ? GetValueAsDate() : For is not null ? GetValueFromModel() : null;

        static Exception GetInvalidDateTypeException(Type modelType) =>
            new NotSupportedException($"Cannot convert '{modelType.FullName}' to a date.");

        DateOnly? GetValueAsDate()
        {
            if (_value is null)
            {
                return null;
            }

            var valueType = _value.GetType();
            var dateInputModelConverters = _optionsAccessor.Value.DateInputModelConverters;

            foreach (var converter in dateInputModelConverters)
            {
                if (converter.CanConvertModelType(valueType))
                {
                    return converter.GetDateFromModel(valueType, _value);
                }
            }

            throw GetInvalidDateTypeException(valueType);
        }

        DateOnly? GetValueFromModel()
        {
            Debug.Assert(For is not null);

            var modelValue = For!.Model;
            var underlyingModelType = Nullable.GetUnderlyingType(For.ModelExplorer.ModelType) ?? For.ModelExplorer.ModelType;

            if (modelValue is null)
            {
                return null;
            }

            var dateInputModelConverters = _optionsAccessor.Value.DateInputModelConverters;

            foreach (var converter in dateInputModelConverters)
            {
                if (converter.CanConvertModelType(underlyingModelType))
                {
                    return converter.GetDateFromModel(underlyingModelType, modelValue);
                }
            }

            throw GetInvalidDateTypeException(underlyingModelType);
        }
    }

    private DateInputItems GetFieldsWithErrors(DateInputContext dateInputContext)
    {
        if (dateInputContext.ErrorFields is not null)
        {
            return dateInputContext.ErrorFields.Value;
        }

        if (For is null)
        {
            return DateInputItems.All;
        }

        Debug.Assert(For is not null);
        Debug.Assert(ViewContext is not null);

        var fullName = _modelHelper.GetFullHtmlFieldName(ViewContext, For.Name);

        if (ViewContext.ModelState.TryGetValue(fullName, out var modelState) &&
            _dateInputParseErrorsProvider.TryGetErrorsForModel(modelState, out var parseErrors))
        {
            return parseErrors.GetFieldsWithErrors();
        }

        return DateInputItems.All;
    }
}
