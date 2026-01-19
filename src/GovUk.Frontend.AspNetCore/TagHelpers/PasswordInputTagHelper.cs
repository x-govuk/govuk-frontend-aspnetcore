using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS password input component.
/// </summary>
[HtmlTargetElement(TagName)]
[RestrictChildren(
    PasswordInputLabelTagHelper.TagName,
    PasswordInputHintTagHelper.TagName,
    PasswordInputErrorMessageTagHelper.TagName,
    PasswordInputBeforeInputTagHelper.TagName,
    PasswordInputAfterInputTagHelper.TagName
#if SHORT_TAG_NAMES
    ,
    FormGroupLabelTagHelperBase.ShortTagName,
    FormGroupHintTagHelperBase.ShortTagName,
    FormGroupErrorMessageTagHelperBase.ShortTagName,
    PasswordInputBeforeInputTagHelper.ShortTagName,
    PasswordInputAfterInputTagHelper.ShortTagName
#endif
    )]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.FormGroup)]
public class PasswordInputTagHelper : TagHelper
{
    internal const string TagName = "govuk-password-input";

    internal IReadOnlyCollection<string> AllTagNames => [TagName];

    private const string AttributesPrefix = "input-";
    private const string AutoCompleteAttributeName = "autocomplete";
    private const string ButtonClassAttributeName = "button-class";
    private const string DescribedByAttributeName = "described-by";
    private const string DisabledAttributeName = "disabled";
    private const string ForAttributeName = "for";
    private const string IdAttributeName = "id";
    private const string IgnoreModelStateErrorsAttributeName = "ignore-modelstate-errors";
    private const string LabelClassAttributeName = "label-class";
    private const string NameAttributeName = "name";
    private const string ReadOnlyAttributeName = "readonly";
    private const string ValueAttributeName = "value";

    private readonly IComponentGenerator _componentGenerator;
    private readonly IModelHelper _modelHelper;
    private readonly HtmlEncoder _encoder;

    private string? _value;
    private bool _valueSpecified;

    /// <summary>
    /// Creates a new <see cref="PasswordInputTagHelper"/>.
    /// </summary>
    public PasswordInputTagHelper(IComponentGenerator componentGenerator, HtmlEncoder encoder) :
        this(componentGenerator, new DefaultModelHelper(), encoder)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);
        ArgumentNullException.ThrowIfNull(encoder);
    }

    internal PasswordInputTagHelper(IComponentGenerator componentGenerator, IModelHelper modelHelper, HtmlEncoder encoder)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);
        ArgumentNullException.ThrowIfNull(modelHelper);
        ArgumentNullException.ThrowIfNull(encoder);

        _componentGenerator = componentGenerator;
        _modelHelper = modelHelper;
        _encoder = encoder;
    }

    /// <summary>
    /// The <c>autocomplete</c> attribute for the generated <c>input</c> element.
    /// </summary>
    [HtmlAttributeName(AutoCompleteAttributeName)]
    public string? AutoComplete { get; set; }

    /// <summary>
    /// Additional classes for the generated <c>button</c> element.
    /// </summary>
    [HtmlAttributeName(ButtonClassAttributeName)]
    public string? ButtonClass { get; set; }

    /// <summary>
    /// One or more element IDs to add to the <c>aria-describedby</c> attribute of the generated <c>input</c> element.
    /// </summary>
    [HtmlAttributeName(DescribedByAttributeName)]
    public string? DescribedBy { get; set; }

    /// <summary>
    /// Whether the <c>disabled</c> attribute should be added to the generated <c>input</c> element.
    /// </summary>
    [HtmlAttributeName(DisabledAttributeName)]
    public bool? Disabled { get; set; }

    /// <summary>
    /// An expression to be evaluated against the current model.
    /// </summary>
    [HtmlAttributeName(ForAttributeName)]
    public ModelExpression? For { get; set; }

    /// <summary>
    /// The <c>id</c> attribute for the generated <c>input</c> element.
    /// </summary>
    /// <remarks>
    /// If not specified then a value is generated from the <c>name</c> attribute.
    /// </remarks>
    [HtmlAttributeName(IdAttributeName)]
    public string? Id { get; set; }

    /// <summary>
    /// Whether the <see cref="ModelStateEntry.Errors"/> for the <see cref="For"/> expression should be used
    /// to deduce an error message.
    /// </summary>
    /// <remarks>
    /// <para>When there are multiple errors in the <see cref="ModelErrorCollection"/> the first is used.</para>
    /// </remarks>
    [HtmlAttributeName(IgnoreModelStateErrorsAttributeName)]
    public bool? IgnoreModelStateErrors { get; set; }

    /// <summary>
    /// Additional attributes to add to the generated <c>input</c> element.
    /// </summary>
    [HtmlAttributeName(DictionaryAttributePrefix = AttributesPrefix)]
    public IDictionary<string, string?> InputAttributes { get; set; } = new Dictionary<string, string?>();

    /// <summary>
    /// Additional classes for the generated <c>label</c> element.
    /// </summary>
    [HtmlAttributeName(LabelClassAttributeName)]
    public string? LabelClass { get; set; }

    /// <summary>
    /// The <c>name</c> attribute for the generated <c>input</c> element.
    /// </summary>
    /// <remarks>
    /// Required unless <see cref="FormGroupTagHelperBase.For"/> is specified.
    /// </remarks>
    [HtmlAttributeName(NameAttributeName)]
    public string? Name { get; set; }

    /// <summary>
    /// Whether the <c>readonly</c> attribute should be added to the generated <c>input</c> element.
    /// </summary>
    [HtmlAttributeName(ReadOnlyAttributeName)]
    public bool? ReadOnly { get; set; }

    /// <summary>
    /// The <c>value</c> attribute for the generated <c>input</c> element.
    /// </summary>
    /// <remarks>
    /// If not specified and <see cref="FormGroupTagHelperBase.For"/> is not <c>null</c> then the value
    /// for the specified model expression will be used.
    /// </remarks>
    [HtmlAttributeName(ValueAttributeName)]
    public string? Value
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
        ArgumentNullException.ThrowIfNull(context);

        context.SetContextItem(new PasswordInputContext());
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        if (ViewContext is null)
        {
            throw new InvalidOperationException($"{nameof(ViewContext)} must be set.");
        }

        var passwordInputContext = context.GetContextItem<PasswordInputContext>();

        using (context.SetScopedContextItem(passwordInputContext))
        using (context.SetScopedContextItem(typeof(FormGroupContext3), passwordInputContext))
        {
            _ = await output.GetChildContentAsync();
        }

        var name = ResolveName();
        var id = ResolveId(name);
        var value = ResolveValue();
        var labelOptions = passwordInputContext.GetLabelOptions(For, ViewContext!, _modelHelper, id, ForAttributeName);
        var hintOptions = passwordInputContext.GetHintOptions(For, _modelHelper);
        var errorMessageOptions = passwordInputContext.GetErrorMessageOptions(For, ViewContext!, _modelHelper, IgnoreModelStateErrors);

        if (LabelClass is not null)
        {
            labelOptions.Classes = labelOptions.Classes.AppendCssClasses(LabelClass);
        }

        var formGroupAttributes = new AttributeCollection(output.Attributes);
        formGroupAttributes.Remove("class", out var formGroupClasses);
        var formGroupOptions = new PasswordInputOptionsFormGroup
        {
            BeforeInput = passwordInputContext.BeforeInput is TemplateString beforeInput ?
                new PasswordInputOptionsBeforeInput()
                {
                    Text = null,
                    Html = beforeInput
                } :
                null,
            AfterInput = passwordInputContext.AfterInput is TemplateString afterInput ?
                new PasswordInputOptionsAfterInput()
                {
                    Text = null,
                    Html = afterInput
                } :
                null,
            Attributes = formGroupAttributes,
            Classes = formGroupClasses
        };

        var buttonOptions = new PasswordInputOptionsButton { Classes = ButtonClass };

        var attributes = new AttributeCollection(InputAttributes);
        attributes.Remove("class", out var classes);

        if (ReadOnly == true)
        {
            attributes.AddBoolean("readonly");
        }

        var component = await _componentGenerator.GeneratePasswordInputAsync(new PasswordInputOptions
        {
            Id = id,
            Name = name,
            Value = value,
            Disabled = Disabled,
            DescribedBy = DescribedBy,
            Label = labelOptions,
            Hint = hintOptions,
            ErrorMessage = errorMessageOptions,
            FormGroup = formGroupOptions,
            Classes = classes,
            AutoComplete = AutoComplete,
            Attributes = attributes,
            ShowPasswordText = null,  // TODO
            HidePasswordText = null,  // TODO
            ShowPasswordAriaLabelText = null,  // TODO
            HidePasswordAriaLabelText = null,  // TODO
            PasswordShownAnnouncementText = null,  // TODO
            PasswordHiddenAnnouncementText = null,  // TODO
            Button = buttonOptions
        });

        component.ApplyToTagHelper(output);

        if (errorMessageOptions is not null)
        {
            Debug.Assert(errorMessageOptions.Html is not null);
            var containerErrorContext = ViewContext.HttpContext.GetContainerErrorContext();
            containerErrorContext.AddError(errorMessageOptions.Html.Value, href: "#" + id);
        }
    }

    private string ResolveId(string name) =>
        Id ?? TagBuilder.CreateSanitizedId(name, Constants.IdAttributeDotReplacement);

    private string ResolveName()
    {
        return Name is null && For is null
            ? throw ExceptionHelper.AtLeastOneOfAttributesMustBeProvided(
                NameAttributeName,
                ForAttributeName)
            : Name ?? _modelHelper.GetFullHtmlFieldName(ViewContext!, For!.Name);
    }

    private string? ResolveValue()
    {
        return _valueSpecified ? _value : For is not null ? _modelHelper.GetModelValue(ViewContext!, For.ModelExplorer, For.Name) : null;
    }
}
