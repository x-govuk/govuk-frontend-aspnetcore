using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class ButtonTagHelperInitializer : ITagHelperInitializer<ButtonTagHelper>
{
    private readonly IOptions<GovUkFrontendOptions> _optionsAccessor;

    public ButtonTagHelperInitializer(IOptions<GovUkFrontendOptions> optionsAccessor)
    {
        ArgumentNullException.ThrowIfNull(optionsAccessor);
        _optionsAccessor = optionsAccessor;
    }

    public void Initialize(ButtonTagHelper helper, ViewContext context)
    {
        ArgumentNullException.ThrowIfNull(helper);

        helper.PreventDoubleClick ??= _optionsAccessor.Value.DefaultButtonPreventDoubleClick;
    }
}
