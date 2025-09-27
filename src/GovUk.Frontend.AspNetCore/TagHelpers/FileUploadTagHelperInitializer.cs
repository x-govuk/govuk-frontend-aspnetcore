using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class FileUploadTagHelperInitializer : ITagHelperInitializer<FileUploadTagHelper>
{
    private readonly IOptions<GovUkFrontendOptions> _optionsAccessor;

    public FileUploadTagHelperInitializer(IOptions<GovUkFrontendOptions> optionsAccessor)
    {
        ArgumentNullException.ThrowIfNull(optionsAccessor);
        _optionsAccessor = optionsAccessor;
    }

    public void Initialize(FileUploadTagHelper helper, ViewContext context)
    {
        ArgumentNullException.ThrowIfNull(helper);

        helper.JavaScriptEnhancements ??= _optionsAccessor.Value.DefaultFileUploadJavaScriptEnhancements;
    }
}
