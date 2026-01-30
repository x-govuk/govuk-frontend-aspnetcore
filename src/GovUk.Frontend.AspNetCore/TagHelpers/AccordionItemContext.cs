using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class AccordionItemContext
{
    public (AttributeCollection Attributes, TemplateString Content)? Heading { get; private set; }
    public (AttributeCollection Attributes, TemplateString Content)? Summary { get; private set; }
    public (AttributeCollection Attributes, TemplateString Content)? Content { get; private set; }

    public void SetHeading(AttributeCollection attributes, TemplateString content)
    {
        ArgumentNullException.ThrowIfNull(attributes);
        ArgumentNullException.ThrowIfNull(content);

        if (Heading is not null)
        {
            throw new InvalidOperationException(
                $"Only one <{AccordionItemHeadingTagHelper.TagName}> is permitted for each <{AccordionItemTagHelper.TagName}>.");
        }

        if (Summary is not null)
        {
            throw new InvalidOperationException(
                $"<{AccordionItemHeadingTagHelper.TagName}> must be specified before <{AccordionItemSummaryTagHelper.TagName}>.");
        }

        if (Content is not null)
        {
            throw new InvalidOperationException(
                $"<{AccordionItemHeadingTagHelper.TagName}> must be specified before <{AccordionItemContentTagHelper.TagName}>.");
        }

        Heading = (attributes, content);
    }

    public void SetSummary(AttributeCollection attributes, TemplateString content)
    {
        ArgumentNullException.ThrowIfNull(attributes);
        ArgumentNullException.ThrowIfNull(content);

        if (Summary is not null)
        {
            throw new InvalidOperationException(
                $"Only one <{AccordionItemSummaryTagHelper.TagName}> is permitted for each <{AccordionItemTagHelper.TagName}>.");
        }

        if (Content is not null)
        {
            throw new InvalidOperationException(
                $"<{AccordionItemSummaryTagHelper.TagName}> must be specified before <{AccordionItemContentTagHelper.TagName}>.");
        }

        Summary = (attributes, content);
    }

    public void SetContent(AttributeCollection attributes, TemplateString content)
    {
        ArgumentNullException.ThrowIfNull(attributes);
        ArgumentNullException.ThrowIfNull(content);

        if (Content is not null)
        {
            throw new InvalidOperationException(
                $"Only one <{AccordionItemContentTagHelper.TagName}> is permitted for each <{AccordionItemTagHelper.TagName}>.");
        }

        Content = (attributes, content);
    }

    public void ThrowIfIncomplete()
    {
        if (Heading is null)
        {
            throw ExceptionHelper.AChildElementMustBeProvided(AccordionItemHeadingTagHelper.TagName);
        }

        if (Content is null)
        {
            throw ExceptionHelper.AChildElementMustBeProvided(AccordionItemContentTagHelper.TagName);
        }
    }
}
