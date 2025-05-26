using GovUk.Frontend.AspNetCore.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.Blazor;

public class BlazorComponentGenerator(HtmlRenderer htmlRenderer)
{
    public async Task<IHtmlContent> GenerateWarningTextAsync(WarningTextOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var parameters = ParameterView.FromDictionary(
            new Dictionary<string, object?>()
            {
                { "Options", options }
            });

        return await htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            var output = await htmlRenderer.RenderComponentAsync<WarningText>(parameters);
            return new HtmlString(output.ToHtmlString());
        });
    }
}
