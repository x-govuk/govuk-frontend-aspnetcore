using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;

namespace GovUk.Frontend.AspNetCore.Validation;

/// <summary>
/// Supplies the adapters MVC needs to localize the library's validation attributes; it leaves the error message of
/// any attribute without an adapter as it is.
/// </summary>
internal sealed class GovUkFrontendValidationAttributeAdapterProvider(IValidationAttributeAdapterProvider inner)
    : IValidationAttributeAdapterProvider
{
    public IAttributeAdapter? GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer? stringLocalizer) =>
        inner.GetAttributeAdapter(attribute, stringLocalizer) ?? attribute switch
        {
            CharacterCountAttribute characterCount =>
                new LimitAttributeAdapter<CharacterCountAttribute>(characterCount, characterCount.MaxLength, stringLocalizer),
            MaxWordsAttribute maxWords =>
                new LimitAttributeAdapter<MaxWordsAttribute>(maxWords, maxWords.Words, stringLocalizer),
            _ => null
        };
}
