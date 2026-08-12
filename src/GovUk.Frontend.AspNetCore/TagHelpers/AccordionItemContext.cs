using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class AccordionItemContext(string itemTagName)
{
    private (AttributeCollection Attributes, IHtmlContent Content, string TagName)? _heading;
    private (AttributeCollection Attributes, IHtmlContent Content, string TagName)? _summary;
    private (AttributeCollection Attributes, IHtmlContent Content, string TagName)? _content;

    public (AttributeCollection Attributes, IHtmlContent Content)? Heading =>
        _heading is var (attributes, content, _) ? (attributes, content) : null;

    public (AttributeCollection Attributes, IHtmlContent Content)? Summary =>
        _summary is var (attributes, content, _) ? (attributes, content) : null;

    public (AttributeCollection Attributes, IHtmlContent Content)? Content =>
        _content is var (attributes, content, _) ? (attributes, content) : null;

    public void SetHeading(AttributeCollection attributes, IHtmlContent content, string tagName)
    {
        ArgumentNullException.ThrowIfNull(attributes);
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(tagName);

        if (_heading is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                AccordionItemHeadingTagHelper.AllTagNames,
                itemTagName);
        }

        if (_summary is var (_, _, summaryTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, summaryTagName);
        }

        if (_content is var (_, _, contentTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, contentTagName);
        }

        _heading = (attributes, content, tagName);
    }

    public void SetSummary(AttributeCollection attributes, IHtmlContent content, string tagName)
    {
        ArgumentNullException.ThrowIfNull(attributes);
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(tagName);

        if (_summary is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                AccordionItemSummaryTagHelper.AllTagNames,
                itemTagName);
        }

        if (_content is var (_, _, contentTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, contentTagName);
        }

        _summary = (attributes, content, tagName);
    }

    public void SetContent(AttributeCollection attributes, IHtmlContent content, string tagName)
    {
        ArgumentNullException.ThrowIfNull(attributes);
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(tagName);

        if (_content is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                AccordionItemContentTagHelper.AllTagNames,
                itemTagName);
        }

        _content = (attributes, content, tagName);
    }

    public void ThrowIfIncomplete()
    {
        if (_heading is null)
        {
            throw ExceptionHelper.AChildElementMustBeProvided(AccordionItemHeadingTagHelper.AllTagNames);
        }

        if (_content is null)
        {
            throw ExceptionHelper.AChildElementMustBeProvided(AccordionItemContentTagHelper.AllTagNames);
        }
    }
}
