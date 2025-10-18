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
            if (baseType is { IsGenericType: true } && baseType.GetGenericTypeDefinition() == typeof(TagHelperTestBase<>))
            {
                var tagHelperType = baseType.GetGenericArguments()[0];
                var tagHelperInfo = TagHelperInfo.GetTagHelperInfo(tagHelperType);
                return testCases.SelectMany(tc => tagHelperInfo.TagNames.Select(i => new TagHelperTestCase(i.TagName, i.ParentTagName, (XunitTestCase)tc))).ToArray();
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
            if (baseType is { IsGenericType: true } && baseType.GetGenericTypeDefinition() == typeof(TagHelperTestBase<>))
            {
                var tagHelperType = baseType.GetGenericArguments()[0];
                var tagHelperInfo = TagHelperInfo.GetTagHelperInfo(tagHelperType);
                return testCases.SelectMany(tc => tagHelperInfo.TagNames.Select(i => new TagHelperTestCase(i.TagName, i.ParentTagName, (XunitTestCase)tc))).ToArray();
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

    public TagHelperTestCase(string tagName, string? parentTagName, XunitTestCase baseTestCase) :
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
        TagName = tagName;
        ParentTagName = parentTagName;
    }

    public string? TagName { get; private set; }

    public string? ParentTagName { get; private set; }

    public ValueTask<RunSummary> Run(
        ExplicitOption explicitOption,
        IMessageBus messageBus,
        object?[] constructorArguments,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource) =>
            TagHelperTestCaseRunner.Instance.Run(
                TagName!,
                ParentTagName,
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

        TagName = info.GetValue<string>(nameof(TagName))!;
        ParentTagName = info.GetValue<string?>(nameof(ParentTagName));
    }

    protected override void Serialize(IXunitSerializationInfo info)
    {
        base.Serialize(info);

        info.AddValue(nameof(TagName), TagName);
        info.AddValue(nameof(ParentTagName), ParentTagName);
    }
}

public class TagHelperTestCaseRunnerContext(
    string tagName,
    string? parentTagName,
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
    public string TagName { get; } = tagName;

    public string? ParentTagName { get; } = parentTagName;
}

public class TagHelperTestCaseRunner : XunitTestCaseRunnerBase<TagHelperTestCaseRunnerContext, IXunitTestCase, IXunitTest>
{
    public static TagHelperTestCaseRunner Instance { get; } = new();

    public async ValueTask<RunSummary> Run(
        string tagName,
        string? parentTagName,
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
            tagName,
            parentTagName,
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
            ctxt.TagName,
            ctxt.ParentTagName,
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
    string tagName,
    string? parentTagName,
    IXunitTest test,
    IMessageBus messageBus,
    ExplicitOption explicitOption,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource,
    IReadOnlyCollection<IBeforeAfterTestAttribute> beforeAfterAttributes,
    object?[] constructorArguments) : XunitTestRunnerContext(test, messageBus, explicitOption, aggregator, cancellationTokenSource, beforeAfterAttributes, constructorArguments)
{
    public string TagName { get; } = tagName;

    public string? ParentTagName { get; } = parentTagName;
}

public class TagHelperTestRunner : XunitTestRunnerBase<TagHelperTestRunnerContext, IXunitTest>
{
    public static TagHelperTestRunner Instance { get; } = new();

    public async ValueTask<RunSummary> Run(
        string tagName,
        string? parentTagName,
        IXunitTest test,
        IMessageBus messageBus,
        object?[] constructorArguments,
        ExplicitOption explicitOption,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource,
        IReadOnlyCollection<IBeforeAfterTestAttribute> beforeAfterAttributes)
    {
        await using var ctxt = new TagHelperTestRunnerContext(
            tagName,
            parentTagName,
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
        var setParentTagNameMethod = result.Instance!.GetType().GetProperty("ParentTagName", BindingFlags.Instance | BindingFlags.Public)!.SetMethod!;

        setTagNameMethod.Invoke(result.Instance, [ctxt.TagName]);
        setParentTagNameMethod.Invoke(result.Instance, [ctxt.ParentTagName]);

        return result;
    }
}

file record TagHelperInfo((string TagName, string? ParentTagName)[] TagNames)
{
    private static readonly Dictionary<Type, TagHelperInfo> _tagInfoByType = [];

    public static TagHelperInfo GetTagHelperInfo(Type tagHelperType)
    {
        if (!_tagInfoByType.TryGetValue(tagHelperType, out var tagInfo))
        {
            var tagHelperAttributes = tagHelperType.GetCustomAttributes<HtmlTargetElementAttribute>(inherit: true).ToArray();

            tagInfo = new(tagHelperAttributes.Select(attr => (attr.Tag, (string?)attr.ParentTag)).ToArray());
            _tagInfoByType[tagHelperType] = tagInfo;
        }

        return tagInfo;
    }
}
