using System.Text;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GovUk.Frontend.AspNetCore.Docs.Pages;

public class Index(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider) : PageModel
{
    private const string ExamplesPathPrefix = "/Examples/";
    private const string ExampleNameSuffix = "Example";

    // Words that shouldn't be lower-cased when they appear in the middle of a name.
    private static readonly Dictionary<string, string> _casingOverrides = new(StringComparer.Ordinal)
    {
        { "Javascript", "JavaScript" }
    };

    public IReadOnlyList<ExampleGroup> ExampleGroups { get; private set; } = [];

    public void OnGet()
    {
        ExampleGroups = actionDescriptorCollectionProvider.ActionDescriptors.Items
            .OfType<PageActionDescriptor>()
            .Where(d => d.ViewEnginePath.StartsWith(ExamplesPathPrefix, StringComparison.Ordinal))
            .DistinctBy(d => d.ViewEnginePath, StringComparer.Ordinal)
            .Select(d => (Segments: d.ViewEnginePath[ExamplesPathPrefix.Length..].Split('/'), d.ViewEnginePath))
            .Where(p => p.Segments.Length == 2)
            .GroupBy(p => p.Segments[0], StringComparer.Ordinal)
            .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
            .Select(g => new ExampleGroup(
                Humanize(g.Key),
                g.Select(p => new Example(Humanize(p.Segments[1]), p.ViewEnginePath))
                    .OrderBy(e => e.Name, StringComparer.OrdinalIgnoreCase)
                    .ToArray()))
            .ToArray();
    }

    // Turns a Pascal-cased page or folder name into sentence-cased text
    // e.g. DateInputWithFieldsetExample -> 'Date input with fieldset'.
    private static string Humanize(string name)
    {
        if (name.Length > ExampleNameSuffix.Length && name.EndsWith(ExampleNameSuffix, StringComparison.Ordinal))
        {
            name = name[..^ExampleNameSuffix.Length];
        }

        var result = new StringBuilder(name.Length + 8);
        var wordStart = 0;

        for (var i = 1; i <= name.Length; i++)
        {
            if (i < name.Length && !char.IsUpper(name[i]))
            {
                continue;
            }

            var word = name[wordStart..i];
            wordStart = i;

            if (result.Length > 0)
            {
                result.Append(' ');
                result.Append(_casingOverrides.GetValueOrDefault(word, word.ToLowerInvariant()));
            }
            else
            {
                result.Append(word);
            }
        }

        return result.ToString();
    }
}

public record ExampleGroup(string Name, IReadOnlyList<Example> Examples);

public record Example(string Name, string PagePath);
