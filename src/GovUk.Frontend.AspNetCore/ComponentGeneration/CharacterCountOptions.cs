using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public record CharacterCountOptions
{
    public TemplateString? Id { get; set; }
    public TemplateString? Name { get; set; }
    public int? Rows { get; set; }
    public TemplateString? Value { get; set; }
    [JsonPropertyName("maxlength")]
    public int? MaxLength { get; set; }
    [JsonPropertyName("maxwords")]
    public int? MaxWords { get; set; }
    public decimal? Threshold { get; set; }
    public LabelOptions? Label { get; set; }
    public HintOptions? Hint { get; set; }
    public ErrorMessageOptions? ErrorMessage { get; set; }
    public CharacterCountOptionsFormGroup? FormGroup { get; set; }
    public TemplateString? Classes { get; set; }
    public bool? Spellcheck { get; set; }
    public AttributeCollection? Attributes { get; set; }
    public CharacterCountCountOptionsMessage? CountMessage { get; set; }
    public string? TextareaDescriptionText { get; set; }
    public CharacterCountOptionsCharactersUnderLimitText? CharactersUnderLimitText { get; set; }
    public string? CharactersAtLimitText { get; set; }
    public CharacterCountOptionsCharactersOverLimitText? CharactersOverLimitText { get; set; }
    public CharacterCountOptionsWordsUnderLimitText? WordsUnderLimitText { get; set; }
    public string? WordsAtLimitText { get; set; }
    public CharacterCountOptionsWordsOverLimitText? WordsOverLimitText { get; set; }
}

public record CharacterCountCountOptionsMessage
{
    public TemplateString? Classes { get; set; }
    [NonStandardParameter]
    public AttributeCollection? Attributes { get; set; }
}

public record CharacterCountOptionsCharactersUnderLimitText
{
    public string? Other { get; set; }
    [NonStandardParameter]
    public string? Zero { get; set; }
    public string? One { get; set; }
    [NonStandardParameter]
    public string? Two { get; set; }
    [NonStandardParameter]
    public string? Few { get; set; }
    [NonStandardParameter]
    public string? Many { get; set; }
}

public record CharacterCountOptionsCharactersOverLimitText
{
    public string? Other { get; set; }
    [NonStandardParameter]
    public string? Zero { get; set; }
    public string? One { get; set; }
    [NonStandardParameter]
    public string? Two { get; set; }
    [NonStandardParameter]
    public string? Few { get; set; }
    [NonStandardParameter]
    public string? Many { get; set; }
}

public record CharacterCountOptionsWordsUnderLimitText
{
    public string? Other { get; set; }
    [NonStandardParameter]
    public string? Zero { get; set; }
    public string? One { get; set; }
    [NonStandardParameter]
    public string? Two { get; set; }
    [NonStandardParameter]
    public string? Few { get; set; }
    [NonStandardParameter]
    public string? Many { get; set; }
}

public record CharacterCountOptionsWordsOverLimitText
{
    public string? Other { get; set; }
    [NonStandardParameter]
    public string? Zero { get; set; }
    public string? One { get; set; }
    [NonStandardParameter]
    public string? Two { get; set; }
    [NonStandardParameter]
    public string? Few { get; set; }
    [NonStandardParameter]
    public string? Many { get; set; }
}

public record CharacterCountOptionsFormGroup : FormGroupOptions
{
    public CharacterCountOptionsBeforeInput? BeforeInput { get; set; }
    public CharacterCountOptionsAfterInput? AfterInput { get; set; }
}

public record CharacterCountOptionsBeforeInput
{
    public string? Text { get; set; }
    public IHtmlContent? Html { get; set; }
    [NonStandardParameter]
    public AttributeCollection? Attributes { get; set; }
}

public record CharacterCountOptionsAfterInput
{
    public string? Text { get; set; }
    public IHtmlContent? Html { get; set; }
    [NonStandardParameter]
    public AttributeCollection? Attributes { get; set; }
}
