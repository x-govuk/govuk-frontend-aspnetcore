using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class CheckboxesItemContext : FormGroupItemContext
{
    protected override string ConditionalTagName => CheckboxesItemConditionalTagHelper.TagName;

    protected override string HintTagName => CheckboxesItemHintTagHelper.TagName;

    protected override string ItemTagName => CheckboxesItemTagHelper.TagName;

    public CheckboxesOptionsItemConditional? GetConditionalOptions() =>
        Conditional is { } conditional ?
            new CheckboxesOptionsItemConditional { Attributes = conditional.Attributes, Html = conditional.Html } :
            null;
}
