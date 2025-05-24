using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace GovUk.Frontend.AspNetCore.ModelBinding;

internal class GovUkFrontendMetadataDetailsProvider : IDisplayMetadataProvider
{
    public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
    {
        var dateOnlyMetadataAttribute = context.Attributes.OfType<DateInputAttribute>().FirstOrDefault();

        if (dateOnlyMetadataAttribute is not null)
        {
            var dateInputMetadata = new DateInputModelMetadata()
            {
                ErrorMessagePrefix = dateOnlyMetadataAttribute.ErrorMessagePrefix,
                ItemTypes = dateOnlyMetadataAttribute.ItemTypes
            };

            context.DisplayMetadata.AdditionalValues.Add(typeof(DateInputModelMetadata), dateInputMetadata);
        }
    }
}
