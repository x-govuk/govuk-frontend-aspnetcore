using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Xunit.Sdk;
using Xunit.v3;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

[XunitTestCaseDiscoverer(typeof(TagHelperFactDiscoverer))]
public class FactAttribute(
    [CallerFilePath] string? sourceFilePath = null,
    [CallerLineNumber] int sourceLineNumber = -1) :
    Xunit.FactAttribute(sourceFilePath, sourceLineNumber);

[XunitTestCaseDiscoverer(typeof(TagHelperTheoryDiscoverer))]
public class TheoryAttribute(
    [CallerFilePath] string? sourceFilePath = null,
    [CallerLineNumber] int sourceLineNumber = -1) :
    Xunit.TheoryAttribute(sourceFilePath, sourceLineNumber);

public class TagHelperFactDiscoverer : FactDiscoverer
{
    public override async ValueTask<IReadOnlyCollection<IXunitTestCase>> Discover(
        ITestFrameworkDiscoveryOptions discoveryOptions,
        IXunitTestMethod testMethod,
        IFactAttribute factAttribute)
    {
        var testCases = (await base.Discover(discoveryOptions, testMethod, factAttribute)).ToArray();

        var baseType = testMethod.TestClass.Class.BaseType;
        while (baseType is not null)
        {
            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(TagHelperTestBase<>))
            {
                var tagHelperType = baseType.GetGenericArguments()[0];
                return TagHelperInfo.CreateTestCasesForEveryTagName(tagHelperType, testCases);
            }

            baseType = baseType.BaseType;
        }

        return testCases;
    }
}

public class TagHelperTheoryDiscoverer : TheoryDiscoverer
{
    public override async ValueTask<IReadOnlyCollection<IXunitTestCase>> Discover(
        ITestFrameworkDiscoveryOptions discoveryOptions,
        IXunitTestMethod testMethod,
        IFactAttribute factAttribute)
    {
        var testCases = (await base.Discover(discoveryOptions, testMethod, factAttribute)).ToArray();

        var baseType = testMethod.TestClass.Class.BaseType;
        while (baseType is not null)
        {
            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(TagHelperTestBase<>))
            {
                var tagHelperType = baseType.GetGenericArguments()[0];
                return TagHelperInfo.CreateTestCasesForEveryTagName(tagHelperType, testCases);
            }

            baseType = baseType.BaseType;
        }

        return testCases;
    }
}

public class TagHelperTestCase : XunitTestCase, ISelfExecutingXunitTestCase
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
    public TagHelperTestCase() { }

    public TagHelperTestCase(
        TagHelperTestCaseInfo tagHelperTestCaseInfo,
        XunitTestCase baseTestCase) :
        base(
            baseTestCase.TestMethod,
            baseTestCase.TestCaseDisplayName,
            baseTestCase.UniqueID,
            baseTestCase.Explicit,
            baseTestCase.SkipExceptions,
            baseTestCase.SkipReason,
            baseTestCase.SkipType,
            baseTestCase.SkipUnless,
            baseTestCase.SkipWhen,
            baseTestCase.Traits,
            baseTestCase.TestMethodArguments,
            baseTestCase.SourceFilePath,
            baseTestCase.SourceLineNumber,
            baseTestCase.Timeout)
    {
        TagHelperTestCaseInfo = tagHelperTestCaseInfo;
    }

    public TagHelperTestCaseInfo? TagHelperTestCaseInfo { get; set; }

    public ValueTask<RunSummary> Run(
        ExplicitOption explicitOption,
        IMessageBus messageBus,
        object?[] constructorArguments,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource) =>
            TagHelperTestCaseRunner.Instance.Run(
                TagHelperTestCaseInfo!,
                this,
                messageBus,
                aggregator.Clone(),
                cancellationTokenSource,
                TestCaseDisplayName,
                SkipReason,
                explicitOption,
                constructorArguments);

    protected override void Deserialize(IXunitSerializationInfo info)
    {
        base.Deserialize(info);

        var tagName = info.GetValue<string>(nameof(TagHelperTestCaseInfo.TagName))!;
        var parentTagName = info.GetValue<string?>(nameof(TagHelperTestCaseInfo.ParentTagName));
        var primaryTagName = info.GetValue<string>(nameof(TagHelperTestCaseInfo.PrimaryTagName))!;
        var shortTagName = info.GetValue<string?>(nameof(TagHelperTestCaseInfo.ShortTagName));
        var allTagNames = info.GetValue<string[]>(nameof(TagHelperTestCaseInfo.AllTagNames))!;
        var allParentTagNames = info.GetValue<string[]>(nameof(TagHelperTestCaseInfo.AllParentTagNames))!;
        TagHelperTestCaseInfo = new TagHelperTestCaseInfo(
            tagName,
            parentTagName,
            primaryTagName,
            shortTagName,
            allTagNames,
            allParentTagNames);
    }

    protected override void Serialize(IXunitSerializationInfo info)
    {
        base.Serialize(info);

        info.AddValue(nameof(TagHelperTestCaseInfo.TagName), TagHelperTestCaseInfo!.TagName);
        info.AddValue(nameof(TagHelperTestCaseInfo.ParentTagName), TagHelperTestCaseInfo.ParentTagName);
        info.AddValue(nameof(TagHelperTestCaseInfo.PrimaryTagName), TagHelperTestCaseInfo.PrimaryTagName);
        info.AddValue(nameof(TagHelperTestCaseInfo.ShortTagName), TagHelperTestCaseInfo.ShortTagName);
        info.AddValue(nameof(TagHelperTestCaseInfo.AllTagNames), TagHelperTestCaseInfo.AllTagNames.ToArray());
        info.AddValue(nameof(TagHelperTestCaseInfo.AllParentTagNames), TagHelperTestCaseInfo.AllParentTagNames.ToArray());
    }
}

public class TagHelperTestCaseRunnerContext(
    TagHelperTestCaseInfo tagHelperTestCaseInfo,
    IXunitTestCase testCase,
    IReadOnlyCollection<IXunitTest> tests,
    IMessageBus messageBus,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource,
    string displayName,
    string? skipReason,
    ExplicitOption explicitOption,
    object?[] constructorArguments) : XunitTestCaseRunnerBaseContext<IXunitTestCase, IXunitTest>(testCase, tests, messageBus, aggregator, cancellationTokenSource, displayName, skipReason, explicitOption, constructorArguments)
{
    public TagHelperTestCaseInfo TagHelperTestCaseInfo => tagHelperTestCaseInfo;
}

public class TagHelperTestCaseRunner : XunitTestCaseRunnerBase<TagHelperTestCaseRunnerContext, IXunitTestCase, IXunitTest>
{
    public static TagHelperTestCaseRunner Instance { get; } = new();

    public async ValueTask<RunSummary> Run(
        TagHelperTestCaseInfo tagHelperTestCaseInfo,
        IXunitTestCase testCase,
        IMessageBus messageBus,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource,
        string displayName,
        string? skipReason,
        ExplicitOption explicitOption,
        object?[] constructorArguments)
    {
        // See XunitRunnerHelper.RunXunitTestCase

        var tests = await aggregator.RunAsync(testCase.CreateTests, []);

        if (aggregator.ToException() is Exception ex)
        {
#pragma warning disable IDE0046
            if (ex.Message.StartsWith(DynamicSkipToken.Value, StringComparison.Ordinal))
            {
                return XunitRunnerHelper.SkipTestCases(
                    messageBus,
                    cancellationTokenSource,
                    [testCase],
                    ex.Message[DynamicSkipToken.Value.Length..],
                    sendTestCaseMessages: false
                );
            }
            else
            {
                return testCase.SkipExceptions?.Contains(ex.GetType()) == true
                    ? XunitRunnerHelper.SkipTestCases(
                        messageBus,
                        cancellationTokenSource,
                        [testCase],
                        ex.Message.Length != 0 ? ex.Message : string.Format(CultureInfo.CurrentCulture, "Exception of type '{0}' was thrown", ex.GetType().SafeName()),
                        sendTestCaseMessages: false
                    )
                    : XunitRunnerHelper.FailTestCases(
                        messageBus,
                        cancellationTokenSource,
                        [testCase],
                        ex,
                        sendTestCaseMessages: false
                    );
            }
#pragma warning restore IDE0046
        }

        await using var ctxt = new TagHelperTestCaseRunnerContext(
            tagHelperTestCaseInfo,
            testCase,
            tests,
            messageBus,
            aggregator,
            cancellationTokenSource,
            displayName,
            skipReason,
            explicitOption,
            constructorArguments);

        await ctxt.InitializeAsync();

        return await Run(ctxt);
    }

    protected override ValueTask<RunSummary> RunTest(TagHelperTestCaseRunnerContext ctxt, IXunitTest test)
    {
        return TagHelperTestRunner.Instance.Run(
            ctxt.TagHelperTestCaseInfo,
            test,
            ctxt.MessageBus,
            ctxt.ConstructorArguments,
            ctxt.ExplicitOption,
            ctxt.Aggregator.Clone(),
            ctxt.CancellationTokenSource,
            ctxt.BeforeAfterTestAttributes);
    }
}

public class TagHelperTestRunnerContext(
    TagHelperTestCaseInfo tagHelperTestCaseInfo,
    IXunitTest test,
    IMessageBus messageBus,
    ExplicitOption explicitOption,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource,
    IReadOnlyCollection<IBeforeAfterTestAttribute> beforeAfterAttributes,
    object?[] constructorArguments) : XunitTestRunnerContext(test, messageBus, explicitOption, aggregator, cancellationTokenSource, beforeAfterAttributes, constructorArguments)
{
    public TagHelperTestCaseInfo TagHelperTestCaseInfo => tagHelperTestCaseInfo;
}

public class TagHelperTestRunner : XunitTestRunnerBase<TagHelperTestRunnerContext, IXunitTest>
{
    public static TagHelperTestRunner Instance { get; } = new();

    public async ValueTask<RunSummary> Run(
        TagHelperTestCaseInfo tagHelperTestCaseInfo,
        IXunitTest test,
        IMessageBus messageBus,
        object?[] constructorArguments,
        ExplicitOption explicitOption,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource,
        IReadOnlyCollection<IBeforeAfterTestAttribute> beforeAfterAttributes)
    {
        await using var ctxt = new TagHelperTestRunnerContext(
            tagHelperTestCaseInfo,
            test,
            messageBus,
            explicitOption,
            aggregator,
            cancellationTokenSource,
            beforeAfterAttributes,
            constructorArguments
        );
        await ctxt.InitializeAsync();

        return await Run(ctxt);
    }

    protected override async ValueTask<(object? Instance, SynchronizationContext? SyncContext, ExecutionContext? ExecutionContext)> CreateTestClassInstance(
        TagHelperTestRunnerContext ctxt)
    {
        var result = await base.CreateTestClassInstance(ctxt);

        var setTagNameMethod = result.Instance!.GetType().GetProperty("TagName", BindingFlags.Instance | BindingFlags.Public)!.SetMethod!;
        var setPrimaryTagNameMethod = result.Instance!.GetType().GetProperty("PrimaryTagName", BindingFlags.Instance | BindingFlags.Public)!.SetMethod!;
        var setShortTagNameMethod = result.Instance!.GetType().GetProperty("ShortTagName", BindingFlags.Instance | BindingFlags.Public)!.SetMethod!;
        var setParentTagNameMethod = result.Instance!.GetType().GetProperty("ParentTagName", BindingFlags.Instance | BindingFlags.Public)!.SetMethod!;
        var setAllTagNamesMethod = result.Instance!.GetType().GetProperty("AllTagNames", BindingFlags.Instance | BindingFlags.Public)!.SetMethod!;
        var setAllParentTagNamesMethod = result.Instance!.GetType().GetProperty("AllParentTagNames", BindingFlags.Instance | BindingFlags.Public)!.SetMethod!;

        setTagNameMethod.Invoke(result.Instance, [ctxt.TagHelperTestCaseInfo.TagName]);
        setParentTagNameMethod.Invoke(result.Instance, [ctxt.TagHelperTestCaseInfo.ParentTagName]);
        setPrimaryTagNameMethod.Invoke(result.Instance, [ctxt.TagHelperTestCaseInfo.PrimaryTagName]);
        setShortTagNameMethod.Invoke(result.Instance, [ctxt.TagHelperTestCaseInfo.ShortTagName]);
        setAllTagNamesMethod.Invoke(result.Instance, [ctxt.TagHelperTestCaseInfo.AllTagNames]);
        setAllParentTagNamesMethod.Invoke(result.Instance, [ctxt.TagHelperTestCaseInfo.AllParentTagNames]);

        return result;
    }
}

file record TagHelperInfo((string TagName, string? ParentTagName)[] TagNames, string PrimaryTagName, string? ShortTagName)
{
    private static readonly Dictionary<Type, TagHelperInfo> _tagInfoByType = [];

    public static TagHelperInfo GetTagHelperInfo(Type tagHelperType)
    {
        if (!_tagInfoByType.TryGetValue(tagHelperType, out var tagInfo))
        {
            var tagHelperAttributes = tagHelperType.GetCustomAttributes<HtmlTargetElementAttribute>(inherit: true).ToArray();

            var primaryTagName = tagHelperType.GetField("TagName", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null) as string
                ?? throw new InvalidOperationException($"Tag helper type '{tagHelperType.FullName}' does not have a public static field 'TagName'.");

            var shortTagName = tagHelperType.GetField("ShortTagName", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null) as string;

            tagInfo = new(tagHelperAttributes.Select(attr => (attr.Tag, (string?)attr.ParentTag)).ToArray(), primaryTagName, shortTagName);
            _tagInfoByType[tagHelperType] = tagInfo;
        }

        return tagInfo;
    }

    public static IReadOnlyCollection<IXunitTestCase> CreateTestCasesForEveryTagName(
        Type tagHelperType,
        IEnumerable<IXunitTestCase> baseTestCases)
    {
        var tagHelperInfo = GetTagHelperInfo(tagHelperType);

        var allTagNames = tagHelperInfo.TagNames.Select(t => t.TagName).Distinct().OrderByGovUkPrefixedFirst().ToArray();
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        var allParentTagNames = tagHelperInfo.TagNames.Select(t => t.ParentTagName!).Where(t => t is not null).Distinct().OrderByGovUkPrefixedFirst().ToArray();

        return tagHelperInfo.TagNames
            .SelectMany(i => baseTestCases.Select(tc =>
            {
                var tagHelperTestCaseInfo = new TagHelperTestCaseInfo(
                    i.TagName,
                    i.ParentTagName,
                    PrimaryTagName: tagHelperInfo.PrimaryTagName,
                    ShortTagName: tagHelperInfo.ShortTagName,
                    AllTagNames: allTagNames,
                    AllParentTagNames: allParentTagNames);

                return new TagHelperTestCase(tagHelperTestCaseInfo, (XunitTestCase)tc);
            }))
            .ToArray();
    }
}

public record TagHelperTestCaseInfo(
    string TagName,
    string? ParentTagName,
    string PrimaryTagName,
    string? ShortTagName,
    IReadOnlyCollection<string> AllTagNames,
    IReadOnlyCollection<string> AllParentTagNames);

file static class Extensions
{
    public static IEnumerable<string> OrderByGovUkPrefixedFirst(this IEnumerable<string> source) =>
        source.OrderBy(s => s.StartsWith("govuk-", StringComparison.Ordinal) ? 0 : 1).ThenBy(s => s, StringComparer.Ordinal);
}
