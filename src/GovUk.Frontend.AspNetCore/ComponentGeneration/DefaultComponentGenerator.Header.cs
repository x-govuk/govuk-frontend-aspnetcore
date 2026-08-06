using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual ValueTask<GovUkComponent> GenerateHeaderAsync(HeaderOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var logoContent = new HtmlContentBuilder();

        logoContent.AppendHtml(GenerateLogo(new LogoOptions
        {
            Classes = "govuk-header__logotype",
            AriaLabelText = "GOV.UK"
        }));

        if (!options.ProductName.IsEmpty())
        {
            var productNameSpan = new HtmlTag("span", attrs => attrs
                .WithClasses("govuk-header__product-name"));
            productNameSpan.InnerHtml.AppendHtml(options.ProductName);
            logoContent.AppendHtml(productNameSpan);
        }

        return GenerateGenericHeaderAsync(new GenericHeaderOptions
        {
            Namespace = "govuk",
            Url = options.HomePageUrl ?? "//gov.uk",
            LogoHtml = logoContent.Snapshot(),
            ContainerClasses = options.ContainerClasses,
            ContainerAttributes = options.ContainerAttributes,
            Classes = options.Classes,
            Attributes = options.Attributes,
            Html = options.Html
        });
    }
}
