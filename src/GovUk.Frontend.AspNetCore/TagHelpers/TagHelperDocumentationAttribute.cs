namespace GovUk.Frontend.AspNetCore.TagHelpers;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
internal sealed class TagHelperDocumentationAttribute : Attribute
{
    public TagHelperDocumentationAttribute()
    {
    }

    /// <param name="tagName">
    /// The tag name this documentation is for, for tag helpers that target more than one element.
    /// </param>
    public TagHelperDocumentationAttribute(string tagName)
    {
        ArgumentNullException.ThrowIfNull(tagName);
        TagName = tagName;
    }

    public string? TagName { get; }

    public string? ContentDescription { get; set; }
}
