namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual ValueTask<GovUkComponent> GenerateHeaderAsync(HeaderOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var headerTag = new HtmlTag("div", attrs => attrs
            .WithClasses("govuk-header", options.Classes)
            .With(options.Attributes));

        var containerTag = new HtmlTag("div", attrs => attrs
            .WithClasses("govuk-header__container", options.ContainerClasses ?? "govuk-width-container")
            .With(options.ContainerAttributes));

        var logoDiv = new HtmlTag("div", attrs => attrs.WithClasses("govuk-header__logo"));

        var logoLink = new HtmlTag("a", attrs => attrs
            .With("href", options.HomePageUrl ?? "//gov.uk")
            .WithClasses("govuk-header__homepage-link"));

        logoLink.InnerHtml.AppendHtml(GenerateLogo(new LogoOptions
        {
            Classes = "govuk-header__logotype",
            AriaLabelText = "GOV.UK"
        }));

        if (!options.ProductName.IsEmpty())
        {
            var productNameSpan = new HtmlTag("span", attrs => attrs
                .WithClasses("govuk-header__product-name"));
            productNameSpan.InnerHtml.AppendHtml(options.ProductName);
            logoLink.InnerHtml.AppendHtml(productNameSpan);
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
