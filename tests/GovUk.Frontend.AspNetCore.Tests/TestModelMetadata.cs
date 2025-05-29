using System.Reflection;
using GovUk.Frontend.AspNetCore.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GovUk.Frontend.AspNetCore.Tests;

internal class TestModelMetadata : ModelMetadata
{
    private string? _displayName;

    public TestModelMetadata(Type modelType)
        : base(ModelMetadataIdentity.ForType(modelType))
    {
    }

    public TestModelMetadata(ParameterInfo parameter)
        : base(ModelMetadataIdentity.ForParameter(parameter))
    {
    }

    public TestModelMetadata(PropertyInfo propertyInfo, Type modelType, Type containerType)
        : base(ModelMetadataIdentity.ForProperty(propertyInfo, modelType, containerType))
    {
    }

    public TestModelMetadata(ModelMetadataIdentity identity)
        : base(identity)
    {
    }

    public DateInputModelMetadata? DateInputData { get; set; }

    public override IReadOnlyDictionary<object, object> AdditionalValues
    {
        get
        {
            var additionalValues = new Dictionary<object, object>();

            if (DateInputData is not null)
            {
                additionalValues[typeof(DateInputModelMetadata)] = DateInputData;
            }

            return additionalValues;
        }
    }

    public override string BinderModelName =>
        throw new NotImplementedException();

    public override Type BinderType =>
        throw new NotImplementedException();

    public override BindingSource BindingSource =>
        throw new NotImplementedException();

    public override bool ConvertEmptyStringToNull =>
        throw new NotImplementedException();

    public override string DataTypeName =>
        throw new NotImplementedException();

    public override string Description =>
        throw new NotImplementedException();

    public override string DisplayFormatString =>
        throw new NotImplementedException();

    public override string DisplayName => _displayName!;

    public override string EditFormatString =>
        throw new NotImplementedException();

    public override ModelMetadata ElementMetadata =>
        throw new NotImplementedException();

    public override IEnumerable<KeyValuePair<EnumGroupAndName, string>> EnumGroupedDisplayNamesAndValues =>
        throw new NotImplementedException();

    public override IReadOnlyDictionary<string, string> EnumNamesAndValues =>
        throw new NotImplementedException();

    public override bool HasNonDefaultEditFormat =>
        throw new NotImplementedException();

    public override bool HideSurroundingHtml =>
        throw new NotImplementedException();

    public override bool HtmlEncode =>
        throw new NotImplementedException();

    public override bool IsBindingAllowed =>
        throw new NotImplementedException();

    public override bool IsBindingRequired =>
        throw new NotImplementedException();

    public override bool IsEnum =>
        throw new NotImplementedException();

    public override bool IsFlagsEnum =>
        throw new NotImplementedException();

    public override bool IsReadOnly =>
        throw new NotImplementedException();

    public override bool IsRequired =>
        throw new NotImplementedException();

    public override ModelBindingMessageProvider ModelBindingMessageProvider =>
        throw new NotImplementedException();

    public override string NullDisplayText =>
        throw new NotImplementedException();

    public override int Order =>
        throw new NotImplementedException();

    public override string Placeholder =>
        throw new NotImplementedException();

    public override ModelPropertyCollection Properties =>
        throw new NotImplementedException();

    public override IPropertyFilterProvider PropertyFilterProvider =>
        throw new NotImplementedException();

    public override bool ShowForDisplay =>
        throw new NotImplementedException();

    public override bool ShowForEdit =>
        throw new NotImplementedException();

    public override string SimpleDisplayProperty =>
        throw new NotImplementedException();

    public override string TemplateHint =>
        throw new NotImplementedException();

    public override IPropertyValidationFilter PropertyValidationFilter =>
        throw new NotImplementedException();

    public override bool ValidateChildren =>
        throw new NotImplementedException();

    public override IReadOnlyList<object> ValidatorMetadata =>
        throw new NotImplementedException();

    public override Func<object, object> PropertyGetter =>
        throw new NotImplementedException();

    public override Action<object, object?>? PropertySetter =>
        throw new NotImplementedException();

    public override ModelMetadata BoundConstructor =>
        throw new NotImplementedException();

    public override Func<object?[], object>? BoundConstructorInvoker =>
        throw new NotImplementedException();

    public override IReadOnlyList<ModelMetadata> BoundConstructorParameters =>
        throw new NotImplementedException();

    public void SetDisplayName(string? displayName) => _displayName = displayName;
}
