using System.Globalization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    /// <summary>
    /// Generates the GOV.UK logo SVG.
    /// This method can be used by footer, header, and service navigation components.
    /// </summary>
    protected virtual IHtmlContent GenerateLogo(LogoOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        // Default options
        var rebrand = options.Rebrand ?? false;
        var useLogotype = options.UseLogotype ?? true;
        var useTudorCrown = options.UseTudorCrown ?? true;

        // Force use of Tudor Crown if rebrand flag is on
        if (rebrand)
        {
            useTudorCrown = true;
        }

        // Calculate how wide the logo needs to be for the given parameters
        var svgWidth = useTudorCrown ? 32 : 36;
        if (useLogotype && !rebrand)
        {
            svgWidth += 116;
        }
        else if (useLogotype && rebrand)
        {
            svgWidth += 130;
        }

        var svgTag = new HtmlTag("svg", attrs => attrs
            .WithBoolean("focusable")  // Will be set to false below
            .With("role", !string.IsNullOrEmpty(options.AriaLabelText) ? "img" : "presentation")
            .With("xmlns", "http://www.w3.org/2000/svg")
            .With("viewBox", $"0 0 {svgWidth * 2} 60")
            .With("height", "30")
            .With("width", svgWidth.ToString(CultureInfo.InvariantCulture))
            .With("fill", "currentcolor")
            .WithClasses(options.Classes)
            .With(options.Attributes));

        // Set focusable to false
        svgTag.Attributes.Set("focusable", "false");

        if (!string.IsNullOrEmpty(options.AriaLabelText))
        {
            svgTag.Attributes.Set("aria-label", options.AriaLabelText);
            var titleTag = new HtmlTag("title");
            titleTag.InnerHtml.AppendHtml(options.AriaLabelText);
            svgTag.InnerHtml.AppendHtml(titleTag);
        }

        // Add crown
        if (useTudorCrown)
        {
            svgTag.InnerHtml.AppendHtml(GetTudorCrown());
        }
        else
        {
            svgTag.InnerHtml.AppendHtml(GetStEdwardsCrown());
        }

        // Add logotype if needed
        if (useLogotype)
        {
            if (rebrand)
            {
                svgTag.InnerHtml.AppendHtml(GetDotLogotype());
            }
            else
            {
                svgTag.InnerHtml.AppendHtml(GetLogotype(useTudorCrown));
            }
        }

        return svgTag;
    }

    private static IHtmlContent GetTudorCrown()
    {
        var gTag = new HtmlTag("g");
        gTag.InnerHtml.AppendHtml(CreateCircle(20, 17.6, 3.7));
        gTag.InnerHtml.AppendHtml(CreateCircle(10.2, 23.5, 3.7));
        gTag.InnerHtml.AppendHtml(CreateCircle(3.7, 33.2, 3.7));
        gTag.InnerHtml.AppendHtml(CreateCircle(31.7, 30.6, 3.7));
        gTag.InnerHtml.AppendHtml(CreateCircle(43.3, 17.6, 3.7));
        gTag.InnerHtml.AppendHtml(CreateCircle(53.2, 23.5, 3.7));
        gTag.InnerHtml.AppendHtml(CreateCircle(59.7, 33.2, 3.7));
        gTag.InnerHtml.AppendHtml(CreateCircle(31.7, 30.6, 3.7));

        var pathData = "M33.1,9.8c.2-.1.3-.3.5-.5l4.6,2.4v-6.8l-4.6,1.5c-.1-.2-.3-.3-.5-.5l1.9-5.9h-6.7l1.9,5.9c-.2.1-.3.3-.5.5l-4.6-1.5v6.8l4.6-2.4c.1.2.3.3.5.5l-2.6,8c-.9,2.8,1.2,5.7,4.1,5.7h0c3,0,5.1-2.9,4.1-5.7l-2.6-8ZM37,37.9s-3.4,3.8-4.1,6.1c2.2,0,4.2-.5,6.4-2.8l-.7,8.5c-2-2.8-4.4-4.1-5.7-3.8.1,3.1.5,6.7,5.8,7.2,3.7.3,6.7-1.5,7-3.8.4-2.6-2-4.3-3.7-1.6-1.4-4.5,2.4-6.1,4.9-3.2-1.9-4.5-1.8-7.7,2.4-10.9,3,4,2.6,7.3-1.2,11.1,2.4-1.3,6.2,0,4,4.6-1.2-2.8-3.7-2.2-4.2.2-.3,1.7.7,3.7,3,4.2,1.9.3,4.7-.9,7-5.9-1.3,0-2.4.7-3.9,1.7l2.4-8c.6,2.3,1.4,3.7,2.2,4.5.6-1.6.5-2.8,0-5.3l5,1.8c-2.6,3.6-5.2,8.7-7.3,17.5-7.4-1.1-15.7-1.7-24.5-1.7h0c-8.8,0-17.1.6-24.5,1.7-2.1-8.9-4.7-13.9-7.3-17.5l5-1.8c-.5,2.5-.6,3.7,0,5.3.8-.8,1.6-2.3,2.2-4.5l2.4,8c-1.5-1-2.6-1.7-3.9-1.7,2.3,5,5.2,6.2,7,5.9,2.3-.4,3.3-2.4,3-4.2-.5-2.4-3-3.1-4.2-.2-2.2-4.6,1.6-6,4-4.6-3.7-3.7-4.2-7.1-1.2-11.1,4.2,3.2,4.3,6.4,2.4,10.9,2.5-2.8,6.3-1.3,4.9,3.2-1.8-2.7-4.1-1-3.7,1.6.3,2.3,3.3,4.1,7,3.8,5.4-.5,5.7-4.2,5.8-7.2-1.3-.2-3.7,1-5.7,3.8l-.7-8.5c2.2,2.3,4.2,2.7,6.4,2.8-.7-2.3-4.1-6.1-4.1-6.1h10.6,0Z";
        var pathTag = CreatePath(pathData);

        gTag.InnerHtml.AppendHtml(pathTag);
        return gTag;
    }

    private static IHtmlContent GetStEdwardsCrown()
    {
        var pathData = "M13.4,22.3c2,.8,4.2-.2,5-2s-.2-4.2-2-5c-2-.8-4.2.2-5,2-.8,2,0,4.2,2,5M4.8,27.3c2,.8,4.2-.2,5-2s-.2-4.2-2-5c-2-.8-4.2.2-5,2-1,2,0,4.2,2,5M2.2,36.9c2,.8,4.2-.2,5-2,.8-2-.2-4.2-2-5-2-.8-4.2.2-5,2-.8,2,0,4.2,2,5M23,25.3c2,.8,4.2-.2,5-2s-.2-4.2-2-5c-2-.8-4.2.2-5,2s0,4.2,2,5M57.8,22.3c-2,.8-4.2-.2-5-2s.2-4.2,2-5c2-.8,4.2.2,5,2,1,2,0,4.2-2,5M66.4,27.3c-2,.8-4.2-.2-5-2s.2-4.2,2-5c2-.8,4.2.2,5,2,1,2,0,4.2-2,5M69,36.9c-2,.8-4.2-.2-5-2-.8-2,.2-4.2,2-5,2-.8,4.2.2,5,2,.8,2,0,4.2-2,5M48.2,25.3c-2,.8-4.2-.2-5-2s.2-4.2,2-5c2-.8,4.2.2,5,2s0,4.2-2,5M37.6,15.5l4.8,2.6v-7.2l-4.8,1.6c-.2-.2-.2-.4-.4-.4s2-6,2-6h-6.8l2,6c-.2.2-.4.2-.4.4-.2.2-4.8-1.4-4.8-1.4v7l4.8-2.6c-.2.2,0,.4.2.6l-2.8,8.4c-.2.4-.2.8-.2,1.4,0,2.2,1.6,4.2,3.8,4.4h1.2c2.2-.4,3.8-2.2,3.8-4.4s0-.8-.2-1.4l-2.8-8.4c.4-.2.6-.4.6-.6M35.6,56.1c9.2,0,17.8.6,25.6,1.8,2.2-9.2,4.8-14.4,7.6-18.2l-5.2-1.8c.6,2.6.6,3.8,0,5.6-.8-.8-1.6-2.4-2.2-4.8l-2.4,8.4c1.6-1,2.8-1.8,4-1.8-2.4,5.2-5.4,6.4-7.2,6-2.4-.4-3.4-2.6-3-4.4.6-2.6,3.2-3.2,4.4-.2,2.4-4.8-1.6-6.2-4.2-4.8,3.8-3.8,4.4-7.2,1.2-11.4-4.4,3.4-4.4,6.6-2.4,11.2-2.6-3-6.6-1.4-5,3.4,1.8-2.8,4.2-1,4,1.6-.4,2.4-3.4,4.2-7.4,4-5.6-.4-6-4.4-6-7.4,1.4-.2,3.8,1,6,4l.8-8.8c-2.2,2.4-4.4,2.8-6.6,2.8.8-2.4,4.2-6.2,4.2-6.2h-11s3.6,4,4.2,6.2c-2.2,0-4.4-.6-6.6-2.8l.8,8.8c2.2-3,4.6-4.2,6-4-.2,3.2-.4,7-6,7.4-3.8.4-7-1.6-7.4-4-.4-2.6,2-4.4,3.8-1.6,1.4-4.8-2.6-6.2-5.2-3.4,2-4.6,2-8-2.4-11.2-3.2,4.2-2.6,7.6,1.2,11.4-2.6-1.4-6.4,0-4.2,4.8,1.2-3,3.8-2.2,4.4.2.4,1.8-.8,3.8-3,4.4-2,.4-5-1-7.4-6,1.4,0,2.6.8,4,1.8l-3-8.4c-.6,2.4-1.4,3.8-2.4,4.8-.6-1.6-.4-3,0-5.6l-5.2,1.8c3,3.8,5.6,9,7.8,18.2,7.6-1,16.4-1.8,25.4-1.8";
        return CreatePath(pathData);
    }

    private static IHtmlContent GetLogotype(bool useTudorCrown)
    {
        var pathData = "M88.6,33.2c0,1.8.2,3.4.6,5s1.2,3,2,4.4c1,1.2,2,2.2,3.4,3s3,1.2,5,1.2,3.4-.2,4.6-.8,2.2-1.4,3-2.2,1.2-1.8,1.6-3c.2-1,.4-2,.4-3v-.4h-10.6v-6.4h18.8v23h-7.4v-5c-.6.8-1.2,1.6-2,2.2-.8.6-1.6,1.2-2.6,1.8-1,.4-2,.8-3.2,1.2s-2.4.4-3.6.4c-3,0-5.8-.6-8-1.6-2.4-1.2-4.4-2.6-6-4.6s-2.8-4.2-3.6-6.8c-.6-2.8-1-5.6-1-8.6s.4-5.8,1.4-8.4,2.2-4.8,4-6.8,3.8-3.4,6.2-4.6c2.4-1.2,5.2-1.6,8.2-1.6s3.8.2,5.6.6c1.8.4,3.4,1.2,4.8,2s2.8,1.8,3.8,3c1.2,1.2,2,2.6,2.8,4l-7.4,4.2c-.4-.8-1-1.8-1.6-2.4-.6-.8-1.2-1.4-2-2s-1.6-1-2.6-1.4-2.2-.4-3.4-.4c-2,0-3.6.4-5,1.2-1.4.8-2.6,1.8-3.4,3-1,1.2-1.6,2.8-2,4.4-.6,1.6-.8,3.8-.8,5.4ZM161.4,24.6c-.8-2.6-2.2-4.8-4-6.8s-3.8-3.4-6.2-4.6c-2.4-1.2-5.2-1.6-8.4-1.6s-5.8.6-8.4,1.6c-2.2,1.2-4.4,2.8-6,4.6-1.8,2-3,4.2-4,6.8-.8,2.6-1.4,5.4-1.4,8.4s.4,5.8,1.4,8.4c.8,2.6,2.2,4.8,4,6.8s3.8,3.4,6.2,4.6c2.4,1.2,5.2,1.6,8.4,1.6s5.8-.6,8.4-1.6c2.4-1.2,4.6-2.6,6.2-4.6,1.8-2,3-4.2,4-6.8.8-2.6,1.4-5.4,1.4-8.4-.2-3-.6-5.8-1.6-8.4h0ZM154,33.2c0,2-.2,3.8-.8,5.4-.4,1.6-1.2,3.2-2.2,4.4s-2.2,2.2-3.4,2.8c-1.4.6-3,1-4.8,1s-3.4-.4-4.8-1-2.6-1.6-3.4-2.8c-1-1.2-1.6-2.6-2.2-4.4-.4-1.6-.8-3.4-.8-5.4v-.2c0-2,.2-3.8.8-5.4.4-1.6,1.2-3.2,2.2-4.4,1-1.2,2.2-2.2,3.4-2.8,1.4-.6,3-1,4.8-1s3.4.4,4.8,1,2.6,1.6,3.4,2.8c1,1.2,1.6,2.6,2.2,4.4.4,1.6.8,3.4.8,5.4v.2ZM177.8,54l-11.8-42h9.4l8,31.4h.2l8-31.4h9.4l-11.8,42h-11.4,0ZM235.4,46.7c1.2,0,2.4-.2,3.4-.6,1-.4,2-.8,2.8-1.6s1.4-1.6,1.8-2.8c.4-1.2.6-2.4.6-4V11.8h8.2v27.2c0,2.4-.4,4.4-1.2,6.2s-2,3.4-3.6,4.8c-1.4,1.4-3.2,2.4-5.4,3-2,.8-4.4,1-6.8,1s-4.8-.4-6.8-1c-2-.8-3.8-1.8-5.4-3-1.6-1.4-2.6-3-3.6-4.8-.8-1.8-1.2-4-1.2-6.2V11.7h8.4v26c0,1.6.2,2.8.6,4,.4,1.2,1,2,1.8,2.8s1.6,1.2,2.8,1.6c1.2.4,2.2.6,3.6.6h0ZM261.4,11.9h8.4v18.2l14.8-18.2h10.4l-14.4,16.8,15.4,25.2h-9.8l-11-18.8-5.4,6v12.8h-8.4V11.9h0ZM206.2,44.2c-3,0-5.4,2.4-5.4,5.4s2.4,5.4,5.4,5.4,5.4-2.4,5.4-5.4-2.4-5.4-5.4-5.4Z";
        
        var pathTag = CreatePath(pathData);
        if (!useTudorCrown)
        {
            pathTag.Attributes.Set("transform", "translate(8 0)");
        }
        return pathTag;
    }

    private static IHtmlContent GetDotLogotype()
    {
        var circleTag = new HtmlTag("circle", attrs => attrs
            .WithClasses("govuk-logo-dot")
            .With("cx", "226")
            .With("cy", "36")
            .With("r", "7.3"))
        {
            TagRenderMode = TagRenderMode.SelfClosing
        };

        var pathData = "M93.94 41.25c.4 1.81 1.2 3.21 2.21 4.62 1 1.4 2.21 2.41 3.61 3.21s3.21 1.2 5.22 1.2 3.61-.4 4.82-1c1.4-.6 2.41-1.4 3.21-2.41.8-1 1.4-2.01 1.61-3.01s.4-2.01.4-3.01v.14h-10.86v-7.02h20.07v24.08h-8.03v-5.56c-.6.8-1.38 1.61-2.19 2.41-.8.8-1.81 1.2-2.81 1.81-1 .4-2.21.8-3.41 1.2s-2.41.4-3.81.4a18.56 18.56 0 0 1-14.65-6.63c-1.6-2.01-3.01-4.41-3.81-7.02s-1.4-5.62-1.4-8.83.4-6.02 1.4-8.83a20.45 20.45 0 0 1 19.46-13.65c3.21 0 4.01.2 5.82.8 1.81.4 3.61 1.2 5.02 2.01 1.61.8 2.81 2.01 4.01 3.21s2.21 2.61 2.81 4.21l-7.63 4.41c-.4-1-1-1.81-1.61-2.61-.6-.8-1.4-1.4-2.21-2.01-.8-.6-1.81-1-2.81-1.4-1-.4-2.21-.4-3.61-.4-2.01 0-3.81.4-5.22 1.2-1.4.8-2.61 1.81-3.61 3.21s-1.61 2.81-2.21 4.62c-.4 1.81-.6 3.71-.6 5.42s.8 5.22.8 5.22Zm57.8-27.9c3.21 0 6.22.6 8.63 1.81 2.41 1.2 4.82 2.81 6.62 4.82S170.2 24.39 171 27s1.4 5.62 1.4 8.83-.4 6.02-1.4 8.83-2.41 5.02-4.01 7.02-4.01 3.61-6.62 4.82-5.42 1.81-8.63 1.81-6.22-.6-8.63-1.81-4.82-2.81-6.42-4.82-3.21-4.41-4.01-7.02-1.4-5.62-1.4-8.83.4-6.02 1.4-8.83 2.41-5.02 4.01-7.02 4.01-3.61 6.42-4.82 5.42-1.81 8.63-1.81Zm0 36.73c1.81 0 3.61-.4 5.02-1s2.61-1.81 3.61-3.01 1.81-2.81 2.21-4.41c.4-1.81.8-3.61.8-5.62 0-2.21-.2-4.21-.8-6.02s-1.2-3.21-2.21-4.62c-1-1.2-2.21-2.21-3.61-3.01s-3.21-1-5.02-1-3.61.4-5.02 1c-1.4.8-2.61 1.81-3.61 3.01s-1.81 2.81-2.21 4.62c-.4 1.81-.8 3.61-.8 5.62 0 2.41.2 4.21.8 6.02.4 1.81 1.2 3.21 2.21 4.41s2.21 2.21 3.61 3.01c1.4.8 3.21 1 5.02 1Zm36.32 7.96-12.24-44.15h9.83l8.43 32.77h.4l8.23-32.77h9.83L200.3 58.04h-12.24Zm74.14-7.96c2.18 0 3.51-.6 3.51-.6 1.2-.6 2.01-1 2.81-1.81s1.4-1.81 1.81-2.81a13 13 0 0 0 .8-4.01V13.9h8.63v28.15c0 2.41-.4 4.62-1.4 6.62-.8 2.01-2.21 3.61-3.61 5.02s-3.41 2.41-5.62 3.21-4.62 1.2-7.02 1.2-5.02-.4-7.02-1.2c-2.21-.8-4.01-1.81-5.62-3.21s-2.81-3.01-3.61-5.02-1.4-4.21-1.4-6.62V13.9h8.63v26.95c0 1.61.2 3.01.8 4.01.4 1.2 1.2 2.21 2.01 2.81.8.8 1.81 1.4 2.81 1.81 0 0 1.34.6 3.51.6Zm34.22-36.18v18.92l15.65-18.92h10.82l-15.03 17.32 16.03 26.83h-10.21l-11.44-20.21-5.62 6.22v13.99h-8.83V13.9";

        var pathTag = CreatePath(pathData);

        var gTag = new HtmlTag("g");
        gTag.InnerHtml.AppendHtml(circleTag);
        gTag.InnerHtml.AppendHtml(pathTag);

        return gTag;
    }

    private static HtmlTag CreateCircle(double cx, double cy, double r)
    {
        return new HtmlTag("circle", attrs => attrs
            .With("cx", cx.ToString(CultureInfo.InvariantCulture))
            .With("cy", cy.ToString(CultureInfo.InvariantCulture))
            .With("r", r.ToString(CultureInfo.InvariantCulture)))
        {
            TagRenderMode = TagRenderMode.SelfClosing
        };
    }

    private static HtmlTag CreatePath(string d)
    {
        return new HtmlTag("path", attrs => attrs.With("d", d))
        {
            TagRenderMode = TagRenderMode.SelfClosing
        };
    }
}

/// <summary>
/// Options for generating the GOV.UK logo.
/// </summary>
internal record LogoOptions
{
    public TemplateString? Classes { get; set; }
    public bool? Rebrand { get; set; }
    public bool? UseLogotype { get; set; }
    public bool? UseTudorCrown { get; set; }
    public string? AriaLabelText { get; set; }
    public AttributeCollection? Attributes { get; set; }
}
