using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
    DateInputHintTagHelper.TagName,
    DateInputErrorMessageTagHelper.TagName,
    DateInputDayTagHelper.TagName,
    DateInputMonthTagHelper.TagName,
    DateInputYearTagHelper.TagName
#if SHORT_TAG_NAMES
    ,
    FormGroupHintTagHelperBase.ShortTagName,
    FormGroupErrorMessageTagHelperBase.ShortTagName
#endif
    )]
[OutputElementHint(ComponentGenerator.FormGroupElement)]
public class DateInputTagHelper : TagHelper
{
    internal const string TagName = "govuk-date-input";

    private const string DefaultDayLabel = "Day";
    private const string DefaultMonthLabel = "Month";
    private const string DefaultYearLabel = "Year";

    private const string AspForAttributeName = "asp-for";
    private const string DateInputAttributesPrefix = "date-input-";
    private const string DisabledAttributeName = "disabled";
    private const string ErrorMessagePrefixAttributeName = "error-message-prefix";
    private const string ForAttributeName = "for";
    private const string IdAttributeName = "id";
    private const string IgnoreModelStateErrorsAttributeName = "ignore-modelstate-errors";
    private const string ItemTypesAttributeName = "item-types";
    private const string NamePrefixAttributeName = "name-prefix";
    private const string ReadOnlyAttributeName = "readonly";
    private const string ValueAttributeName = "value";

    private readonly IComponentGenerator _componentGenerator;
    private readonly IOptions<GovUkFrontendOptions> _optionsAccessor;
    private readonly IModelHelper _modelHelper;
    private readonly HtmlEncoder _encoder;

    private object? _value;
    private bool _valueSpecified;

    /// <summary>
    /// Creates a <see cref="DateInputTagHelper"/>.
    /// </summary>
    public DateInputTagHelper(
        IComponentGenerator componentGenerator,
        IOptions<GovUkFrontendOptions> optionsAccessor,
        HtmlEncoder encoder)
        : this(componentGenerator, optionsAccessor, encoder, modelHelper: new DefaultModelHelper())
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);
        ArgumentNullException.ThrowIfNull(optionsAccessor);
        ArgumentNullException.ThrowIfNull(encoder);
    }

    internal DateInputTagHelper(
        IComponentGenerator componentGenerator,
        IOptions<GovUkFrontendOptions> optionsAccessor,
        HtmlEncoder encoder,
        IModelHelper modelHelper)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);
        ArgumentNullException.ThrowIfNull(optionsAccessor);
        ArgumentNullException.ThrowIfNull(encoder);
        ArgumentNullException.ThrowIfNull(modelHelper);

        _componentGenerator = componentGenerator;
        _optionsAccessor = optionsAccessor;
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
    /// The prefix to use in generated error messages.
    /// </summary>
    [HtmlAttributeName(ErrorMessagePrefixAttributeName)]
    public string? ErrorMessagePrefix { get; set; }

    /// <summary>
    /// An expression to be evaluated against the current model.
    /// </summary>
    [HtmlAttributeName(ForAttributeName)]
    public ModelExpression? For { get; set; }

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
    /// The <see cref="DateInputItemTypes"/> that this date input contains.
    /// </summary>
    /// <remarks>
    /// This is required when creating a partial date input (e.g. a day and month only)
    /// and the value is a <see cref="ValueTuple{T1, T2}"/>.
    /// </remarks>
    [HtmlAttributeName(ItemTypesAttributeName)]
    public DateInputItemTypes? ItemTypes { get; set; }

    /// <summary>
    /// Optional prefix for the <c>name</c> attribute on each item's <c>input</c>.
    /// </summary>
    [HtmlAttributeName(NamePrefixAttributeName)]
    public string? NamePrefix { get; set; }

    /// <summary>
    /// Whether the <c>readonly</c> attribute should be added to the generated <c>input</c> elements.
    /// </summary>
    [HtmlAttributeName(ReadOnlyAttributeName)]
    public bool? ReadOnly { get; set; }

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
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var dateInputContext = context.GetContextItem<DateInputContext>();

        _ = await output.GetChildContentAsync();

        var id = ResolveId();
        var namePrefix = ResolveNamePrefix();
        var itemTypes = ResolveItemTypes();
        var value = ResolveValue(itemTypes);
        var hintOptions = dateInputContext.GetHintOptions(For, _modelHelper);
        var errorMessageOptions = dateInputContext.GetErrorMessageOptions(
            namePrefix,
            ErrorMessagePrefix,
            For,
            ViewContext!,
            _modelHelper,
            IgnoreModelStateErrors);

        var formGroupAttributes = new AttributeCollection(output.Attributes);
        formGroupAttributes.Remove("class", out var formGroupClasses);
        var formGroupOptions = new DateInputFormGroupOptions()
        {
            Attributes = formGroupAttributes,
            Classes = formGroupClasses
        };

        var fieldsetOptions = dateInputContext.Fieldset?.GetFieldsetOptions(_modelHelper);

        var errorItems = GetFieldsWithErrors(dateInputContext);

        List<DateInputOptionsItem> items = [];

        DateInputContextItem? GetContextItem(DateInputItemTypes itemType)
        {
            return dateInputContext.Items.TryGetValue(itemType, out var contextItem) && !itemTypes.HasFlag(itemType)
                ? throw new InvalidOperationException(
                    $"Cannot declare a <{contextItem.TagName}> when the parent's {nameof(DateInputItemTypes)} does not contain {itemType}.")
                : contextItem;
        }

        var dayContextItem = GetContextItem(DateInputItemTypes.Day);
        var monthContextItem = GetContextItem(DateInputItemTypes.Month);
        var yearContextItem = GetContextItem(DateInputItemTypes.Year);

        if (itemTypes.HasFlag(DateInputItemTypes.Day))
        {
            items.Add(CreateDateInputItem(
                getComponentFromValue: date => date?.Day?.ToString(CultureInfo.InvariantCulture),
                defaultLabel: DefaultDayLabel,
                defaultName: DateInputModelBinder.DayInputName,
                defaultClass: "govuk-input--width-2",
                DateInputItemTypes.Day,
                dayContextItem));
        }

        if (itemTypes.HasFlag(DateInputItemTypes.Month))
        {
            items.Add(CreateDateInputItem(
                getComponentFromValue: date => date?.Month?.ToString(CultureInfo.InvariantCulture),
                defaultLabel: DefaultMonthLabel,
                defaultName: DateInputModelBinder.MonthInputName,
                defaultClass: "govuk-input--width-2",
                DateInputItemTypes.Month,
                monthContextItem));
        }

        if (itemTypes.HasFlag(DateInputItemTypes.Year))
        {
            items.Add(CreateDateInputItem(
                getComponentFromValue: date => date?.Year?.ToString(CultureInfo.InvariantCulture),
                defaultLabel: DefaultYearLabel,
                defaultName: DateInputModelBinder.YearInputName,
                defaultClass: "govuk-input--width-4",
                DateInputItemTypes.Year,
                yearContextItem));
        }

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
                .First(i => i.Classes?.ToHtmlString(_encoder).Contains("govuk-input--error", StringComparison.Ordinal) == true)
                .Id;

            var containerErrorContext = ViewContext!.HttpContext.GetContainerErrorContext();
            containerErrorContext.AddError(errorMessageOptions.Html, href: "#" + firstFieldWithError!);
        }

        DateInputOptionsItem CreateDateInputItem(
            Func<DateInputItemValues?, string?> getComponentFromValue,
            string defaultLabel,
            string defaultName,
            string defaultClass,
            DateInputItemTypes errorSource,
            DateInputContextItem? contextItem)
        {
            var haveError = errorMessageOptions is not null;

            var defaultFullName = ModelNames.CreatePropertyModelName(namePrefix, defaultName);
            var itemName = contextItem?.Name ?? defaultFullName;
            var itemId = contextItem?.Id ??
                contextItem?.Name?.ToHtmlString(_encoder) ??
                $"{id}.{defaultName}";
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

            var resolvedAttributes = contextItem?.Attributes?.Clone() ?? [];
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

            if (ReadOnly == true)
            {
                resolvedAttributes.AddBoolean("readonly");
            }

            return new DateInputOptionsItem()
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

                return ViewContext!.ModelState.TryGetValue(modelStateKey, out var modelStateEntry) &&
                    modelStateEntry.AttemptedValue is not null
                    ? modelStateEntry.AttemptedValue
                    : getComponentFromValue(value);
            }
        }
    }

    private string ResolveId()
    {
        return For is null && Id is null
            ? throw ExceptionHelper.AtLeastOneOfAttributesMustBeProvided(ForAttributeName, IdAttributeName)
            : Id ??
            TagBuilder.CreateSanitizedId(
                _modelHelper.GetFullHtmlFieldName(ViewContext!, For!.Name),
                Constants.IdAttributeDotReplacement);
    }

    private string ResolveNamePrefix()
    {
        var resolvedName = For is not null ? _modelHelper.GetFullHtmlFieldName(ViewContext!, For.Name) : null;
        return NamePrefix ?? resolvedName ?? string.Empty;
    }

    private DateInputItemTypes ResolveItemTypes()
    {
        Debug.Assert(ViewContext is not null);

        if (ItemTypes is not null)
        {
            return ItemTypes.Value;
        }

        if (For?.Metadata.TryGetDateInputModelMetadata(out var dateInputModelMetadata) == true &&
            dateInputModelMetadata.ItemTypes is DateInputItemTypes metadataTypes)
        {
            return metadataTypes;
        }

        var valueType = (_valueSpecified ? Value?.GetType() : null) ?? For?.Metadata.ModelType;
        if (valueType is not null)
        {
            valueType = Nullable.GetUnderlyingType(valueType) ?? valueType;
        }

        return valueType is not null &&
            _optionsAccessor.Value.FindDateInputModelConverterForType(valueType)?.DefaultItemTypes is DateInputItemTypes converterDefaultTypes
            ? converterDefaultTypes
            : DateInputItemTypes.DayMonthAndYear;
    }

    private DateInputItemValues? ResolveValue(DateInputItemTypes itemTypes)
    {
        return _valueSpecified ? GetValueAsDate(_value?.GetType(), _value) :
            For is not null ? GetValueAsDate(For.ModelExplorer.ModelType, For!.Model) :
            null;

        DateInputItemValues? GetValueAsDate(Type? valueType, object? value)
        {
            if (valueType is null)
            {
                return null;
            }

            var underlyingType = Nullable.GetUnderlyingType(valueType) ?? valueType;
            var converter = _optionsAccessor.Value.FindDateInputModelConverterForType(underlyingType);

            if (!DateInputModelBinder.SupportedItemTypes.Contains(itemTypes))
            {
                throw new InvalidOperationException($"{nameof(DateInputItemTypes)} combination is not supported.");
            }

            if (value is null)
            {
                return null;
            }

            Debug.Assert(valueType is not null);

            return converter is not null
                ? converter.ConvertFromModel(
                    new DateInputConvertFromModelContext(underlyingType, itemTypes, value))
                : throw new NotSupportedException($"Cannot convert '{underlyingType.FullName}' to a date.");
        }
    }

    private DateInputItemTypes GetFieldsWithErrors(DateInputContext dateInputContext)
    {
        if (dateInputContext.ErrorFields is not null)
        {
            return dateInputContext.ErrorFields.Value;
        }

        var modelName = NamePrefix;

        if (For is not null)
        {
            Debug.Assert(ViewContext is not null);
            modelName = _modelHelper.GetFullHtmlFieldName(ViewContext, For.Name);
        }

        Debug.Assert(modelName is not null);

        var invalidDateException = ViewContext!.ModelState[modelName]?.Errors.FirstOrDefault(e => e.Exception is DateInputParseException)
            ?.Exception as DateInputParseException;

        return invalidDateException?.ParseErrors is DateInputParseErrors parseErrors
            ? parseErrors.GetItemsWithError()
            : DateInputItemTypes.All;
    }
}
