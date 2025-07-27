namespace GovUk.Frontend.AspNetCore.TagHelpers;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
internal sealed class TagHelperDocumentationAttribute : Attribute
{
    public string? ContentDescription { get; set; }
}
