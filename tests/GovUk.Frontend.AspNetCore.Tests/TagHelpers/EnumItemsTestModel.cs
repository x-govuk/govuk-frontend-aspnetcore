using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public enum EnumItemsColour
{
    [Display(Name = "Bright red", Description = "Like a postbox", Order = 2)]
    Red = 1,

    [Display(Order = 1)]
    Blue = 2,

    [Display(Name = "None of these", Order = 3)]
    None = 0,
}

[Flags]
public enum EnumItemsFlags
{
    None = 0,
    First = 1,
    [Display(Name = "Second one", Order = 1)]
    Second = 2,
    Both = First | Second,
    Third = 4,
}

public class EnumItemsTestModel
{
    public EnumItemsColour Colour { get; set; }

    public EnumItemsColour? NullableColour { get; set; }

    public List<EnumItemsColour>? Colours { get; set; }

    public EnumItemsFlags Flags { get; set; }

    public EnumItemsFlags? NullableFlags { get; set; }

    public string? Text { get; set; }

    internal static ModelExpression CreateModelExpression(
        EnumItemsTestModel model,
        string propertyName,
        IServiceProvider? services = null)
    {
        // The MVC metadata provider, so that [Display] attributes on enum members are read.
        var metadataProvider = (services ?? new ServiceCollection().AddMvc().Services.BuildServiceProvider())
            .GetRequiredService<IModelMetadataProvider>();

        var modelExplorer = metadataProvider.GetModelExplorerForType(typeof(EnumItemsTestModel), model)
            .GetExplorerForProperty(propertyName);

        return new ModelExpression(propertyName, modelExplorer);
    }
}
