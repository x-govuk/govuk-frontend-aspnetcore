using Microsoft.Extensions.Logging;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal static partial class Log
{
    [LoggerMessage(LogLevel.Warning, "Attributes are not supported on <{tagName}> and will be ignored.")]
    public static partial void AttributesAreNotSupportedOnTagNameAndWillBeIgnored(this ILogger logger, string tagName);
}
