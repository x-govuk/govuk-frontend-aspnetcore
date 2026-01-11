namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual Task<GovUkComponent> GenerateLabelAsync(LabelOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var content = HtmlOrText(options.Html, options.Text);

        // Only generate label if there is content
        if (content.IsEmpty())
        {
#pragma warning disable VSTHRD003
            return EmptyComponentTask;
#pragma warning restore VSTHRD003
        }

        var label = new HtmlTag("label", attrs => attrs
            .With("for", options.For)
            .WithClasses("govuk-label", options.Classes)
            .With(options.Attributes))
        {
            content
        };

        if (options.IsPageHeading == true)
        {
            var wrapper = new HtmlTag("h1", attrs => attrs
                .WithClasses("govuk-label-wrapper"))
            {
                label
            };

            return GenerateFromHtmlTagAsync(wrapper);
        }

        return GenerateFromHtmlTagAsync(label);
    }
}
