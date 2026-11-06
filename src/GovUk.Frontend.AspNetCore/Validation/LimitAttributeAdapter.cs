using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace GovUk.Frontend.AspNetCore.Validation;

/// <summary>
/// Localizes the error message of an attribute that enforces a limit, with the display name as <c>{0}</c> and the
/// limit as <c>{1}</c>.
/// </summary>
internal sealed class LimitAttributeAdapter<TAttribute>(TAttribute attribute, int limit, IStringLocalizer? stringLocalizer)
    : AttributeAdapterBase<TAttribute>(attribute, stringLocalizer)
    where TAttribute : ValidationAttribute
{
    // These limits are only enforced on the server.
    public override void AddValidation(ClientModelValidationContext context)
    {
    }

    public override string GetErrorMessage(ModelValidationContextBase validationContext)
    {
        ArgumentNullException.ThrowIfNull(validationContext);

        return GetErrorMessage(
            validationContext.ModelMetadata,
            validationContext.ModelMetadata.GetDisplayName(),
            limit);
    }
}
