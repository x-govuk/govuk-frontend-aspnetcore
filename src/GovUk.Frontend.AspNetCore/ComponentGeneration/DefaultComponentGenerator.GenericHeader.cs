namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    // _namespace is a private option which allows the generic header to also be used by the 'main' header.
    private const string DefaultHeaderNamespace = "govuk-generic";

    public virtual ValueTask<GovUkComponent> GenerateGenericHeaderAsync(GenericHeaderOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var @namespace = options.Namespace ?? DefaultHeaderNamespace;

        var headerTag = new HtmlTag("div", attrs => attrs
            .WithClasses($"{@namespace}-header", options.Classes)
            .With(options.Attributes));

        var containerTag = new HtmlTag("div", attrs => attrs
            .WithClasses($"{@namespace}-header__container", options.ContainerClasses ?? "govuk-width-container")
            .With(options.ContainerAttributes));

        var logoDiv = new HtmlTag("div", attrs => attrs
            .WithClasses($"{@namespace}-header__logo")
            .With(options.LogoAttributes));

        var logoLink = new HtmlTag("a", attrs => attrs
            .With("href", options.Url ?? "/")
            .WithClasses($"{@namespace}-header__homepage-link")
            .With(options.LinkAttributes));

        if (!options.LogoHtml.IsEmpty())
        {
            logoLink.InnerHtml.AppendHtml(options.LogoHtml.GetRawHtml());
        }
        else if (!options.LogoText.IsEmpty())
        {
            logoLink.InnerHtml.AppendHtml(options.LogoText);
        }

        logoDiv.InnerHtml.AppendHtml(logoLink);
        containerTag.InnerHtml.AppendHtml(logoDiv);

        if (!options.Html.IsEmpty())
        {
            containerTag.InnerHtml.AppendHtml(options.Html.GetRawHtml());
        }

        headerTag.InnerHtml.AppendHtml(containerTag);

        return GenerateFromHtmlTagAsync(headerTag);
    }
}
