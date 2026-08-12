using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class RadiosItemContext : FormGroupItemContext
{
    protected override string ConditionalTagName => RadiosItemConditionalTagHelper.TagName;

    protected override string HintTagName => RadiosItemHintTagHelper.TagName;

    protected override string ItemTagName => RadiosItemTagHelper.TagName;

    public RadiosOptionsItemConditional? GetConditionalOptions() =>
        Conditional is { } conditional ?
            new RadiosOptionsItemConditional { Attributes = conditional.Attributes, Html = conditional.Html } :
            null;
}
