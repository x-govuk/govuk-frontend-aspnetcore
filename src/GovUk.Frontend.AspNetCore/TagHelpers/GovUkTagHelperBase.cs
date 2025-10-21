using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Base class for tag helpers that generate GDS components.
/// </summary>
public abstract class GovUkTagHelperBase : TagHelper
{
    private protected GovUkTagHelperBase() { }

    private protected abstract string PrimaryTagName { get; }

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        var newContext = context.TryGetContextItem<GovUkTagHelperContext>(out var govUkTagHelperContext)
            ? govUkTagHelperContext.Push(PrimaryTagName)
            : GovUkTagHelperContext.Create(PrimaryTagName);

        context.SetContextItem(newContext);
    }
}
