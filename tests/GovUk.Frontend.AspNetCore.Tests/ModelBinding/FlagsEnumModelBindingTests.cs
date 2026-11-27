using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;

namespace GovUk.Frontend.AspNetCore.Tests.ModelBinding;

/// <summary>
/// <c>&lt;govuk-checkboxes&gt;</c> bound to a single <c>[Flags]</c> enum posts one value per checked item under the same
/// name and relies on MVC's model binding to combine them; these tests catch MVC changing that.
/// </summary>
public class FlagsEnumModelBindingTests
{
    private const string ModelName = "Permissions";

    [Flags]
    public enum Permissions
    {
        None = 0,
        Read = 1,
        Write = 2,
        Delete = 4,
        ReadWrite = Read | Write
    }

    [Theory]
    [MemberData(nameof(ValidCases))]
    public async Task BindModelAsync_WithMultipleValues_CombinesValues(Type modelType, string[] values, Permissions expected)
    {
        // Arrange & Act
        var (result, errors) = await BindAsync(modelType, values);

        // Assert
        Assert.True(result.IsModelSet);
        Assert.Equal(expected, result.Model);
        Assert.Empty(errors);
    }

    public static TheoryData<Type, string[], Permissions> ValidCases()
    {
        var data = new TheoryData<Type, string[], Permissions>();

        foreach (var modelType in new[] { typeof(Permissions), typeof(Permissions?) })
        {
            // Names, as posted by explicitly-declared items
            data.Add(modelType, ["Read", "Write"], Permissions.ReadWrite);
            data.Add(modelType, ["read", "write"], Permissions.ReadWrite);

            // Numbers, as posted by generated items
            data.Add(modelType, ["1", "2"], Permissions.ReadWrite);
            data.Add(modelType, ["1", "2", "4"], Permissions.ReadWrite | Permissions.Delete);

            // Mixed
            data.Add(modelType, ["Read", "4"], Permissions.Read | Permissions.Delete);
            data.Add(modelType, ["Read, Write", "Delete"], Permissions.ReadWrite | Permissions.Delete);
            data.Add(modelType, ["", "Read", "Write"], Permissions.ReadWrite);
        }

        return data;
    }

    [Theory]
    [InlineData(typeof(Permissions), new[] { "Read", "Bogus" }, "The value 'Read,Bogus' is not valid.")]
    [InlineData(typeof(Permissions?), new[] { "Read", "Bogus" }, "The value 'Read,Bogus' is not valid.")]
    [InlineData(typeof(Permissions), new[] { "Read", "8" }, "The value 'Read,8' is invalid.")]
    [InlineData(typeof(Permissions?), new[] { "Read", "8" }, "The value 'Read,8' is invalid.")]
    public async Task BindModelAsync_WithInvalidValue_Fails(Type modelType, string[] values, string expectedError)
    {
        // Arrange & Act
        var (result, errors) = await BindAsync(modelType, values);

        // Assert
        Assert.False(result.IsModelSet);
        Assert.Equal([expectedError], errors);
    }

    [Fact]
    public async Task BindModelAsync_ForNullableWithNoValues_DoesNotSetModel()
    {
        // Arrange & Act
        var (result, errors) = await BindAsync(typeof(Permissions?), []);

        // Assert
        Assert.False(result.IsModelSet);
        Assert.Empty(errors);
    }

    private static async Task<(ModelBindingResult Result, string[] Errors)> BindAsync(Type modelType, string[] values)
    {
        var services = new ServiceCollection();
        services.AddMvc();
        services.AddGovUkFrontend();
        var serviceProvider = services.BuildServiceProvider();

        var metadata = serviceProvider.GetRequiredService<IModelMetadataProvider>().GetMetadataForType(modelType);

        var binder = serviceProvider.GetRequiredService<IModelBinderFactory>().CreateBinder(new ModelBinderFactoryContext
        {
            Metadata = metadata,
            BindingInfo = new BindingInfo { BinderModelName = ModelName },
            CacheToken = null
        });

        var form = values.Length == 0 ?
            new FormCollection([]) :
            new FormCollection(new Dictionary<string, StringValues> { [ModelName] = new(values) });

        var bindingContext = DefaultModelBindingContext.CreateBindingContext(
            new ActionContext(new DefaultHttpContext { RequestServices = serviceProvider }, new RouteData(), new ActionDescriptor()),
            new FormValueProvider(BindingSource.Form, form, CultureInfo.InvariantCulture),
            metadata,
            bindingInfo: null,
            modelName: ModelName);

        await binder.BindModelAsync(bindingContext);

        var errors = bindingContext.ModelState.TryGetValue(ModelName, out var entry) ?
            entry.Errors.Select(e => e.ErrorMessage).ToArray() :
            [];

        return (bindingContext.Result, errors);
    }
}
