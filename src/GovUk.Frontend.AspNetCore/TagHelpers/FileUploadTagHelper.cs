using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using AttributeCollection = GovUk.Frontend.AspNetCore.ComponentGeneration.AttributeCollection;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS File upload component.
/// </summary>
[HtmlTargetElement(TagName)]
[RestrictChildren(
    FileUploadLabelTagHelper.TagName,
    FileUploadHintTagHelper.TagName,
    FileUploadErrorMessageTagHelper.TagName,
    FileUploadBeforeInputTagHelper.TagName,
    FileUploadAfterInputTagHelper.TagName
#if SHORT_TAG_NAMES
    ,
    FormGroupLabelTagHelperBase.ShortTagName,
    FormGroupHintTagHelperBase.ShortTagName,
    FormGroupErrorMessageTagHelperBase.ShortTagName,
    FileUploadBeforeInputTagHelper.ShortTagName,
    FileUploadAfterInputTagHelper.ShortTagName
#endif
    )]
public class FileUploadTagHelper : TagHelper
{
    internal const string TagName = "govuk-file-upload";

    private const string AttributesPrefix = "input-";
    private const string ChooseFilesButtonTextAttributeName = "choose-files-button-text";
    private const string DescribedByAttributeName = "described-by";
    private const string DisabledAttributeName = "disabled";
    private const string DropInstructionTextAttributeName = "drop-instruction-text";
    private const string EnteredDropZoneTextAttributeName = "entered-drop-zone-text";
    private const string ForAttributeName = "for";
    private const string IdAttributeName = "id";
    private const string IgnoreModelStateErrorsAttributeName = "ignore-modelstate-errors";
    private const string JavaScriptEnhancementsAttributeName = "javascript-enhancements";
    private const string LabelClassAttributeName = "label-class";
    private const string LeftDropZoneTextAttributeName = "left-drop-zone-text";
    private const string MultipleAttributeName = "multiple";
    private const string MultipleFilesChosenTextOneAttributeName = "multiple-files-chosen-text-one";
    private const string MultipleFilesChosenTextOtherAttributeName = "multiple-files-chosen-text-other";
    private const string NameAttributeName = "name";
    private const string NoFileChosenTextAttributeName = "no-file-chosen-text";
    private const string WrapperAttributesPrefix = "wrapper-";

    private readonly IComponentGenerator _componentGenerator;
    private readonly IModelHelper _modelHelper;

    /// <summary>
    /// Creates an <see cref="TextInputTagHelper"/>.
    /// </summary>
    public FileUploadTagHelper(IComponentGenerator componentGenerator)
        : this(componentGenerator, modelHelper: new DefaultModelHelper())
    {
    }

    internal FileUploadTagHelper(IComponentGenerator componentGenerator, IModelHelper modelHelper)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);
        ArgumentNullException.ThrowIfNull(modelHelper);

        _componentGenerator = componentGenerator;
        _modelHelper = modelHelper;
    }

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
    /// Whether to enable JavaScript enhancements for the component.
    /// </summary>
    /// <remarks>
    /// The default is set for the application in <see cref="GovUkFrontendOptions.DefaultFileUploadJavaScriptEnhancements"/>.
    /// </remarks>
    [HtmlAttributeName(JavaScriptEnhancementsAttributeName)]
    public bool? JavaScriptEnhancements { get; set; }

    /// <summary>
    /// Additional classes for the generated <c>label</c> element.
    /// </summary>
    [HtmlAttributeName(LabelClassAttributeName)]
    public string? LabelClass { get; set; }

    /// <summary>
    /// The <c>multiple</c> attribute for the generated <c>input</c> element.
    /// </summary>
    [HtmlAttributeName(MultipleAttributeName)]
    public bool? Multiple { get; set; }

    /// <summary>
    /// Text for the button that opens the file picker.
    /// </summary>
    [HtmlAttributeName(ChooseFilesButtonTextAttributeName)]
    public string? ChooseFilesButtonText { get; set; }

    /// <summary>
    /// Text instructing users to drop files in the drop zone.
    /// </summary>
    [HtmlAttributeName(DropInstructionTextAttributeName)]
    public string? DropInstructionText { get; set; }

    /// <summary>
    /// Text announced when a user enters the drop zone while dragging files.
    /// </summary>
    [HtmlAttributeName(EnteredDropZoneTextAttributeName)]
    public string? EnteredDropZoneText { get; set; }

    /// <summary>
    /// Text announced when a user leaves the drop zone while dragging files.
    /// </summary>
    [HtmlAttributeName(LeftDropZoneTextAttributeName)]
    public string? LeftDropZoneText { get; set; }

    /// <summary>
    /// Text shown when exactly one file has been chosen (used when <see cref="Multiple"/> is <c>true</c>).
    /// </summary>
    [HtmlAttributeName(MultipleFilesChosenTextOneAttributeName)]
    public string? MultipleFilesChosenTextOne { get; set; }

    /// <summary>
    /// Text shown when more than one file has been chosen (used when <see cref="Multiple"/> is <c>true</c>).
    /// </summary>
    [HtmlAttributeName(MultipleFilesChosenTextOtherAttributeName)]
    public string? MultipleFilesChosenTextOther { get; set; }

    /// <summary>
    /// Text shown when no file has been chosen.
    /// </summary>
    [HtmlAttributeName(NoFileChosenTextAttributeName)]
    public string? NoFileChosenText { get; set; }

    /// <summary>
    /// The <c>name</c> attribute for the generated <c>input</c> element.
    /// </summary>
    /// <remarks>
    /// Required unless <see cref="For"/> is specified.
    /// </remarks>
    [HtmlAttributeName(NameAttributeName)]
    public string? Name { get; set; }

    /// <summary>
    /// Gets the <see cref="ViewContext"/> of the executing view.
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    [DisallowNull]
    public ViewContext? ViewContext { get; set; }

    /// <summary>
    /// Additional attributes to add to the Javascript enhanced component's wrapper element.
    /// </summary>
    [HtmlAttributeName(DictionaryAttributePrefix = WrapperAttributesPrefix)]
    public IDictionary<string, string?> WrapperAttributes { get; set; } = new Dictionary<string, string?>();

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        var fileUploadContext = new FileUploadContext();
        context.SetContextItem(fileUploadContext);
        context.SetContextItem(typeof(FormGroupContext3), fileUploadContext);
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var fileUploadContext = context.GetContextItem<FileUploadContext>();

        await output.GetChildContentAsync();

        var name = ResolveName();
        var id = ResolveId(name);
        var labelOptions = fileUploadContext.GetLabelOptions(For, ViewContext!, _modelHelper, id, ForAttributeName);
        var hintOptions = fileUploadContext.GetHintOptions(For, _modelHelper);
        var errorMessageOptions = fileUploadContext.GetErrorMessageOptions(For, ViewContext!, _modelHelper, IgnoreModelStateErrors);

        if (LabelClass is not null)
        {
            labelOptions.Classes = labelOptions.Classes.AppendCssClasses(LabelClass);
        }

        var formGroupAttributes = new AttributeCollection(output.Attributes);
        formGroupAttributes.Remove("class", out var formGroupClasses);
        var formGroupOptions = new FileUploadOptionsFormGroup
        {
            BeforeInput = fileUploadContext.BeforeInput is TemplateString beforeInput ?
                new FileUploadOptionsBeforeInput
                {
                    Html = beforeInput.ToHtmlString()
                } :
                null,
            AfterInput = fileUploadContext.AfterInput is TemplateString afterInput ?
                new FileUploadOptionsAfterInput
                {
                    Html = afterInput.ToHtmlString()
                } :
                null,
            Attributes = formGroupAttributes,
            Classes = formGroupClasses
        };

        var wrapperAttributes = new AttributeCollection(WrapperAttributes);
        wrapperAttributes.Remove("class", out var wrapperClasses);

        var attributes = new AttributeCollection(InputAttributes);
        attributes.Remove("class", out var classes);

        var component = await _componentGenerator.GenerateFileUploadAsync(new FileUploadOptions
        {
            Id = id,
            Name = name,
            Disabled = Disabled,
            Multiple = Multiple,
            DescribedBy = DescribedBy,
            Label = labelOptions,
            Hint = hintOptions,
            ErrorMessage = errorMessageOptions,
            FormGroup = formGroupOptions,
            JavaScript = JavaScriptEnhancements,
            ChooseFilesButtonText = ChooseFilesButtonText,
            DropInstructionText = DropInstructionText,
            MultipleFilesChosenText = MultipleFilesChosenTextOne is not null || MultipleFilesChosenTextOther is not null
                ? new FileUploadOptionsMultipleFilesChosenText
                {
                    One = MultipleFilesChosenTextOne,
                    Other = MultipleFilesChosenTextOther
                }
                : null,
            NoFileChosenText = NoFileChosenText,
            EnteredDropZoneText = EnteredDropZoneText,
            LeftDropZoneText = LeftDropZoneText,
            WrapperClasses = wrapperClasses,
            WrapperAttributes = wrapperAttributes,
            Classes = classes,
            Attributes = attributes
        });

        component.ApplyToTagHelper(output);

        if (errorMessageOptions is not null)
        {
            Debug.Assert(errorMessageOptions.Html is not null);
            var containerErrorContext = ViewContext!.HttpContext.GetPageErrorContext();
            containerErrorContext.AddError(errorMessageOptions.Html!, href: "#" + id);
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
}
