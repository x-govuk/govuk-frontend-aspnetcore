namespace GovUk.Frontend.AspNetCore.TagHelpers;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
internal sealed class TagHelperDocumentationAttribute : Attribute
{
    public string? ContentDescription { get; set; }
}
