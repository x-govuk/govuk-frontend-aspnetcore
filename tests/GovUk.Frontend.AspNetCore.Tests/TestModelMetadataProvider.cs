using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace GovUk.Frontend.AspNetCore.Tests;

public class TestModelMetadataProvider : DefaultModelMetadataProvider
{
    public TestModelMetadataProvider() : base(new TestCompositeMetadataDetailsProvider())
    {
    }

    public new TestCompositeMetadataDetailsProvider DetailsProvider => (TestCompositeMetadataDetailsProvider)base.DetailsProvider;
}
