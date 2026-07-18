using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class GenericHeaderContext
{
    public (TemplateString Content, AttributeCollection Attributes, AttributeCollection LinkAttributes)? Logo { get; private set; }

    public void SetLogo(
        TemplateString content,
        AttributeCollection attributes,
        AttributeCollection linkAttributes)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(attributes);
        ArgumentNullException.ThrowIfNull(linkAttributes);

        if (Logo is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                GenericHeaderLogoTagHelper.TagName,
                GenericHeaderTagHelper.TagName);
        }

        Logo = (content, attributes, linkAttributes);
    }

    public void ThrowIfNotComplete()
    {
        if (Logo is null)
        {
            throw ExceptionHelper.AChildElementMustBeProvided(GenericHeaderLogoTagHelper.TagName);
        }
    }
}
