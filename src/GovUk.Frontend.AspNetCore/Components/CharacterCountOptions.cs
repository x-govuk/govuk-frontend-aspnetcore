using System.Text.Json.Serialization;

namespace GovUk.Frontend.AspNetCore.Components;

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
    public TemplateString? TextareaDescriptionText { get; set; }
    public CharacterCountOptionsCharactersUnderLimitText? CharactersUnderLimitText { get; set; }
    public TemplateString? CharactersAtLimitText { get; set; }
    public CharacterCountOptionsCharactersOverLimitText? CharactersOverLimitText { get; set; }
    public CharacterCountOptionsWordsUnderLimitText? WordsUnderLimitText { get; set; }
    public TemplateString? WordsAtLimitText { get; set; }
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
    public TemplateString? Other { get; set; }
    public TemplateString? One { get; set; }
}

public record CharacterCountOptionsCharactersOverLimitText
{
    public TemplateString? Other { get; set; }
    public TemplateString? One { get; set; }

}

public record CharacterCountOptionsWordsUnderLimitText
{
    public TemplateString? Other { get; set; }
    public TemplateString? One { get; set; }
}

public record CharacterCountOptionsWordsOverLimitText
{
    public TemplateString? Other { get; set; }
    public TemplateString? One { get; set; }
}

public record CharacterCountOptionsFormGroup : FormGroupOptions
{
    public CharacterCountOptionsBeforeInput? BeforeInput { get; set; }
    public CharacterCountOptionsAfterInput? AfterInput { get; set; }
}

public record CharacterCountOptionsBeforeInput
{
    public string? Text { get; set; }
    public string? Html { get; set; }
    [NonStandardParameter]
    public AttributeCollection? Attributes { get; set; }
}

public record CharacterCountOptionsAfterInput
{
    public string? Text { get; set; }
    public string? Html { get; set; }
    [NonStandardParameter]
    public AttributeCollection? Attributes { get; set; }
}
