namespace GovUk.Frontend.AspNetCore;

/// <summary>
/// Represents how the content at the end of a GDS service navigation component is aligned.
/// </summary>
public enum ServiceNavigationEndSlotAlign
{
    /// <summary>
    /// The content is displayed underneath the navigation items.
    /// </summary>
    Default = 0,

    /// <summary>
    /// The content is displayed in line with the navigation items, when there's enough space for it.
    /// </summary>
    Inline = 1
}
