namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

/// <summary>
/// Indicates a component parameter whose value is HTML, for parameters that aren't named for it.
/// </summary>
/// <remarks>
/// Parameters whose name ends in <c>Html</c> are recognized without this attribute.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
internal sealed class HtmlParameterAttribute : Attribute
{
}
