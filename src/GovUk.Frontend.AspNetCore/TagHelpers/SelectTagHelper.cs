using GovUk.Frontend.AspNetCore.HtmlGeneration;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS select component.
/// </summary>
[HtmlTargetElement(TagName)]
[RestrictChildren(SelectItemTagHelper.TagName, LabelTagName, HintTagName, ErrorMessageTagName)]
[OutputElementHint(ComponentGenerator.FormGroupElement)]
public class SelectTagHelper : FormGroupTagHelperBase
{
    internal const string ErrorMessageTagName = "govuk-select-error-message";
    internal const string HintTagName = "govuk-select-hint";
    internal const string LabelTagName = "govuk-select-label";
    internal const string TagName = "govuk-select";

    private const string AttributesPrefix = "select-";
    private const string DescribedByAttributeName = "described-by";
    private const string DisabledAttributeName = "disabled";
    private const string IdAttributeName = "id";
    private const string LabelClassAttributeName = "label-class";
    private const string NameAttributeName = "name";

    /// <summary>
    /// Creates a new <see cref="SelectTagHelper"/>.
    /// </summary>
    public SelectTagHelper()
        : this(htmlGenerator: null, modelHelper: null)
    {
    }

    internal SelectTagHelper(IGovUkHtmlGenerator? htmlGenerator = null, IModelHelper? modelHelper = null)
        : base(
              htmlGenerator ?? new ComponentGenerator(),
              modelHelper ?? new DefaultModelHelper())
    {
    }

    /// <summary>
    /// One or more element IDs to add to the <c>aria-describedby</c> attribute of the generated <c>select</c> element.
    /// </summary>
    [HtmlAttributeName(DescribedByAttributeName)]
    public new string? DescribedBy
    {
        get => base.DescribedBy;
        set => base.DescribedBy = value;
    }

    /// <summary>
    /// Whether the <c>disabled</c> attribute should be added to the generated <c>select</c> element.
    /// </summary>
    [HtmlAttributeName(DisabledAttributeName)]
    public bool? Disabled { get; set; }

    /// <summary>
    /// The <c>id</c> attribute for the generated <c>select</c> element.
    /// </summary>
    /// <remarks>
    /// If not specified then a value is generated from the <c>name</c> attribute.
    /// </remarks>
    [HtmlAttributeName(IdAttributeName)]
    public string? Id { get; set; }

    /// <summary>
    /// Additional classes for the generated <c>label</c> element.
    /// </summary>
    [HtmlAttributeName(LabelClassAttributeName)]
    public string? LabelClass { get; set; }

    /// <summary>
    /// The <c>name</c> attribute for the generated <c>select</c> element.
    /// </summary>
    /// <remarks>
    /// Required unless <see cref="FormGroupTagHelperBase.AspFor"/> is specified.
    /// </remarks>
    [HtmlAttributeName(NameAttributeName)]
    public string? Name { get; set; }

    /// <summary>
    /// Additional attributes to add to the generated <c>select</c> element.
    /// </summary>
    [HtmlAttributeName(DictionaryAttributePrefix = AttributesPrefix)]
    public IDictionary<string, string?>? SelectAttributes { get; set; } = new Dictionary<string, string?>();

    private protected override FormGroupContext CreateFormGroupContext() => new SelectContext(For);

    private protected override IHtmlContent GenerateFormGroupContent(
        TagHelperContext tagHelperContext,
        FormGroupContext formGroupContext,
        TagHelperOutput tagHelperOutput,
        IHtmlContent content,
        out bool haveError)
    {
        var selectContext = tagHelperContext.GetContextItem<SelectContext>();

        var contentBuilder = new HtmlContentBuilder();

        var label = GenerateLabel(formGroupContext, LabelClass);
        contentBuilder.AppendHtml(label);

        var hint = GenerateHint(tagHelperContext, formGroupContext);
        if (hint is not null)
        {
            contentBuilder.AppendHtml(hint);
        }

        var errorMessage = GenerateErrorMessage(tagHelperContext, formGroupContext);
        if (errorMessage is not null)
        {
            contentBuilder.AppendHtml(errorMessage);
        }

        haveError = errorMessage is not null;

        var selectTagBuilder = GenerateSelect(haveError);
        contentBuilder.AppendHtml(selectTagBuilder);

        return contentBuilder;

        TagBuilder GenerateSelect(bool haveError)
        {
            var resolvedId = ResolveIdPrefix();
            var resolvedName = ResolveName();

            return Generator.GenerateSelect(
                haveError,
                resolvedId,
                resolvedName,
                DescribedBy,
                Disabled ?? ComponentGenerator.SelectDefaultDisabled,
                selectContext.Items,
                SelectAttributes.ToAttributeDictionary());
        }
    }

    private protected override string ResolveIdPrefix()
    {
        if (Id is not null)
        {
            return Id;
        }

        if (Name is null && For is null)
        {
            throw ExceptionHelper.AtLeastOneOfAttributesMustBeProvided(
                IdAttributeName,
                NameAttributeName,
                AspForAttributeName);
        }

        var resolvedName = ResolveName();

        return TagBuilder.CreateSanitizedId(resolvedName, Constants.IdAttributeDotReplacement);
    }

    private string ResolveName()
    {
        if (Name is null && For is null)
        {
            throw ExceptionHelper.AtLeastOneOfAttributesMustBeProvided(
                NameAttributeName,
                AspForAttributeName);
        }

        return Name ?? ModelHelper.GetFullHtmlFieldName(ViewContext!, For!.Name);
    }
}
