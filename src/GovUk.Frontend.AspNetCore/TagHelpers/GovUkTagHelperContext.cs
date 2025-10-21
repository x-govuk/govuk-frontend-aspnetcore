namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal sealed class GovUkTagHelperContext
{
    private GovUkTagHelperContext(IReadOnlyCollection<string> tagNames)
    {
        ArgumentNullException.ThrowIfNull(tagNames);

        TagNames = tagNames;
    }

    public IReadOnlyCollection<string> TagNames { get; }

    public static GovUkTagHelperContext Create(string tagName)
    {
        ArgumentNullException.ThrowIfNull(tagName);

        return new GovUkTagHelperContext([tagName]);
    }

    public GovUkTagHelperContext Push(string tagName)
    {
        ArgumentNullException.ThrowIfNull(tagName);

        return new GovUkTagHelperContext(TagNames.Prepend(tagName).ToArray());
    }
}
