using System.Text.Encodings.Web;
using System.Text.Unicode;
using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.Tests.ComponentGeneration;

/// <summary>
/// Properties that have to hold for every <see cref="TemplateString"/>, checked exhaustively over a
/// small alphabet rather than by example. The value space is tiny, so the whole cartesian product
/// composed to depth two runs in milliseconds.
/// </summary>
public class TemplateStringPropertyTests
{
    private static readonly string[] Alphabet =
        ["", "a", "&", "<b>", "\"'", "é", "AT&amp;T", "a & b"];

    private static readonly HtmlEncoder DefaultEncoder = HtmlEncoder.Default;

    /// <summary>
    /// An encoder that leaves non-ASCII alone — what a Welsh service would register.
    /// </summary>
    private static readonly HtmlEncoder UnicodeEncoder = HtmlEncoder.Create(UnicodeRanges.All);

    /// <summary>
    /// Every way of building a <see cref="TemplateString"/> holding each alphabet value.
    /// </summary>
    private static IEnumerable<(string Description, TemplateString Value)> Atoms()
    {
        foreach (var s in Alphabet)
        {
            yield return ($"Text({s})", new TemplateString(s));
            yield return ($"Html(Encode({s}))", TemplateString.FromEncoded(DefaultEncoder.Encode(s)));
            yield return ($"Html({s})", TemplateString.FromEncoded(s));
        }
    }

    private static IEnumerable<(string Description, TemplateString Value)> Composed()
    {
        foreach (var (description, value) in Atoms())
        {
            yield return (description, value);
        }

        foreach (var (leftDescription, left) in Atoms())
        {
            foreach (var (rightDescription, right) in Atoms())
            {
                yield return ($"{leftDescription} + {rightDescription}", left + right);
                yield return ($"Join(' ', {leftDescription}, {rightDescription})", TemplateString.Join(" ", left, right));
                yield return ($"$\"{{{leftDescription}}}-{{{rightDescription}}}\"", new TemplateString($"{left}-{right}"));
            }
        }
    }

    [Fact]
    public void Concatenation_DistributesOverRendering()
    {
        // Render(a + b) == Render(a) + Render(b). If this holds, concatenation can't be quietly
        // changing how either operand is treated.
        foreach (var (leftDescription, left) in Atoms())
        {
            foreach (var (rightDescription, right) in Atoms())
            {
                var expected = Render(left) + Render(right);
                var actual = Render(left + right);

                Assert.True(expected == actual, $"{leftDescription} + {rightDescription}: expected '{expected}', got '{actual}'");
            }
        }
    }

    [Fact]
    public void TryGetText_AgreesWithRendering()
    {
        // The invariant that makes TryGetText safe to build keys and tokens from: whenever it
        // succeeds, encoding the text it returns reproduces the rendered HTML exactly.
        foreach (var (description, value) in Composed())
        {
            if (!value.TryGetText(out var text))
            {
                continue;
            }

            Assert.True(
                DefaultEncoder.Encode(text) == Render(value),
                $"{description}: text '{text}' encodes to '{DefaultEncoder.Encode(text)}' but renders as '{Render(value)}'");
        }
    }

    [Fact]
    public void TryGetText_SucceedsForEverythingBuiltFromText()
    {
        foreach (var s in Alphabet)
        {
            Assert.True(new TemplateString(s).TryGetText(out var text));
            Assert.Equal(s, text);
        }

        // ...including once it's been composed.
        Assert.True((new TemplateString("a") + new TemplateString("&")).TryGetText(out var concatenated));
        Assert.Equal("a&", concatenated);

        Assert.True(TemplateString.Join("-", new TemplateString("a"), new TemplateString("&")).TryGetText(out var joined));
        Assert.Equal("a-&", joined);

        Assert.True(new TemplateString($"{new TemplateString("a")}-{new TemplateString("&")}").TryGetText(out var interpolated));
        Assert.Equal("a-&", interpolated);
    }

    [Fact]
    public void TryGetText_FailsForMarkup()
    {
        Assert.False(TemplateString.FromEncoded("<b>bold</b>").TryGetText(out _));
        Assert.False(TemplateString.FromEncoded("&lt;b&gt;").TryGetText(out _));

        // A composite is only text if all of it is.
        Assert.False((new TemplateString("a") + TemplateString.FromEncoded("<b>")).TryGetText(out _));
    }

    [Fact]
    public void Rendering_UsesTheSuppliedEncoder()
    {
        // Composition must not bake in HtmlEncoder.Default, or an app that registers a Unicode-aware
        // encoder gets é in some places and &#xE9; in others.
        foreach (var (description, value) in Composed())
        {
            if (!value.TryGetText(out var text) || !text.Contains('é', StringComparison.Ordinal))
            {
                continue;
            }

            Assert.True(
                Render(value, UnicodeEncoder).Contains('é', StringComparison.Ordinal),
                $"{description}: rendered as '{Render(value, UnicodeEncoder)}'");
        }
    }

    [Fact]
    public void Join_EncodesTheSeparator()
    {
        var joined = TemplateString.Join(" & ", new TemplateString("a"), new TemplateString("b"));

        Assert.Equal("a &amp; b", Render(joined));
    }

    [Fact]
    public void Join_MatchesStringJoinOfTheRenderedParts()
    {
        foreach (var (leftDescription, left) in Atoms())
        {
            foreach (var (rightDescription, right) in Atoms())
            {
                var parts = new[] { left, right }.Where(p => !p.IsEmpty()).Select(p => Render(p));
                var expected = string.Join(DefaultEncoder.Encode("-"), parts);
                var actual = Render(TemplateString.Join("-", left, right));

                Assert.True(expected == actual, $"Join('-', {leftDescription}, {rightDescription}): expected '{expected}', got '{actual}'");
            }
        }
    }

    [Fact]
    public void Equality_AndHashing_AgreeWithRendering()
    {
        // Rendered forms and hashes are computed once; comparing every pair otherwise means rendering
        // each value a few thousand times.
        var all = Composed()
            .Select(v => (v.Description, v.Value, Rendered: Render(v.Value), Hash: v.Value.GetHashCode()))
            .ToArray();

        foreach (var left in all)
        {
            foreach (var right in all)
            {
                var rendersTheSame = left.Rendered == right.Rendered;
                var isEqual = left.Value.Equals(right.Value);

                Assert.True(
                    isEqual == rendersTheSame,
                    $"'{left.Description}' vs '{right.Description}': Equals returned {isEqual} but renders {(rendersTheSame ? "the same" : "differently")}");

                if (isEqual)
                {
                    Assert.True(
                        left.Hash == right.Hash,
                        $"'{left.Description}' equals '{right.Description}' but hashes differently");
                }
            }
        }
    }

    [Fact]
    public void EmptyValues_AreAllEqual()
    {
        Assert.Equal(TemplateString.Empty, new TemplateString(""));
        Assert.Equal(TemplateString.Empty, new TemplateString((string?)null));
        Assert.Equal(TemplateString.Empty, TemplateString.FromEncoded(""));
        Assert.Equal(TemplateString.Empty.GetHashCode(), new TemplateString("").GetHashCode());
    }

    [Fact]
    public void IsHtml_IsTrueWhenAnyPartIsHtml()
    {
        Assert.False(new TemplateString("a").IsHtml);
        Assert.True(TemplateString.FromEncoded("<b>").IsHtml);

        Assert.False((new TemplateString("a") + new TemplateString("b")).IsHtml);
        Assert.True((new TemplateString("a") + TemplateString.FromEncoded("<b>")).IsHtml);
        Assert.True(TemplateString.Join(" ", new TemplateString("a"), TemplateString.FromEncoded("<b>")).IsHtml);
    }

    private static string Render(TemplateString value, HtmlEncoder? encoder = null)
    {
        using var writer = new StringWriter();
        value.WriteTo(writer, encoder ?? DefaultEncoder);
        return writer.ToString();
    }
}
