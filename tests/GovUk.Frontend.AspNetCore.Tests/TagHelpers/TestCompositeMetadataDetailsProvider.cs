using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TestCompositeMetadataDetailsProvider(
    string name,
    string? displayName,
    string? description) : ICompositeMetadataDetailsProvider
{
    public void CreateBindingMetadata(BindingMetadataProviderContext context)
    {
    }

    public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
    {
        if (context.Key.Name != name)
        {
            return;
        }

        context.DisplayMetadata.DisplayName = () => displayName;
        context.DisplayMetadata.Description = () => description;
    }

    public void CreateValidationMetadata(ValidationMetadataProviderContext context)
    {
    }
}
