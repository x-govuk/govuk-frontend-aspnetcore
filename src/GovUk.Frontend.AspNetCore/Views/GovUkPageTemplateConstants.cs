#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1034 // Nested types should not be visible

namespace GovUk.Frontend.AspNetCore.Views;

/// <summary>
/// Constants for the GovUkPageTemplate view.
/// </summary>
public static class GovUkPageTemplateConstants
{
    /// <summary>
    /// The names of the sections supported by the GovUkPageTemplate view.
    /// </summary>
    public static class SectionNames
    {
        public const string BodyEnd = "BodyEnd";
        public const string BodyStart = "BodyStart";
        public const string Container = "Container";
        public const string ContainerEnd = "ContainerEnd";
        public const string ContainerStart = "ContainerStart";
        public const string Footer = "Footer";
        public const string FooterEnd = "FooterEnd";
        public const string FooterStart = "FooterStart";
        public const string GovUkFooter = "GovUkFooter";
        public const string GovUkHeader = "GovUkHeader";
        public const string GovUkServiceNavigation = "GovUkServiceNavigation";
        public const string Head = "Head";
        public const string Header = "Header";
        public const string HeaderEnd = "HeaderEnd";
        public const string HeadIcons = "HeadIcons";
        public const string HeaderStart = "HeaderStart";
        public const string SkipLink = "SkipLink";
    }

    /// <summary>
    /// The keys for the ViewData entries supported by the GovUkPageTemplate view.
    /// </summary>
    public static class ViewDataKeys
    {
        public const string AssetPath = "AssetPath";
        public const string BodyClasses = "BodyClasses";
        public const string BodyAttributes = "BodyAttributes";
        public const string ContainerAttributes = "ContainerAttributes";
        public const string ContainerClasses = "ContainerClasses";
        public const string CspNonce = "CspNonce";
        public const string FooterAttributes = "FooterAttributes";
        public const string FooterClasses = "FooterClasses";
        public const string HeaderAttributes = "HeaderAttributes";
        public const string HeaderClasses = "HeaderClasses";
        public const string MainAttributes = "MainAttributes";
        public const string HtmlClasses = "HtmlClasses";
        public const string HtmlLang = "HtmlLang";
        public const string MainLang = "MainLang";
        public const string MainClasses = "MainClasses";
        public const string OpengraphImageUrl = "OpengraphImageUrl";
        public const string ServiceName = "ServiceName";
        public const string ServiceUrl = "ServiceUrl";
        public const string ThemeColor = "ThemeColor";
        public const string Title = "Title";
        public const string TitleLang = "TitleLang";
    }
}
