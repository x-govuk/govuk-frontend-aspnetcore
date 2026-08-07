# Typing component parameters

Notes for contributors adding or changing a property on an `*Options` record in
`src/GovUk.Frontend.AspNetCore/ComponentGeneration`. The rules here are what the encoding work in
v5 settled on; they exist so a value's type says what it holds, rather than leaving it to be
inferred from how the value happened to be constructed.

## The three types

| Type | Means | Written as |
| --- | --- | --- |
| `string?` | plain text | HTML-encoded |
| `IHtmlContent?` | markup | verbatim |
| `TemplateString?` | either — it knows which | encoded if it holds text, verbatim if it holds markup |

`TemplateString?` is the right answer whenever a value can legitimately be either — content written
inside a tag helper, and attribute values. Where a value can only ever be one of the two, say so with
`string?` or `IHtmlContent?`.

## Deciding which one

### 1. Is it set from a tag helper's inner content?

If a tag helper writes to the parameter from `output.GetChildContentAsync()` or `output.Content`,
the answer is **`TemplateString?`** and you can stop here. Razor content is markup, and
`TemplateString` carries markup or text without needing a second property.

Do this **even where the reference Nunjucks implementation only supports text.** Upstream renders
`divider`, `number` and the select item's text with `{{ }}`, which Nunjucks autoescapes — so those
parameters are text-only upstream. Letting someone write markup inside `<govuk-select-item>` or
`<govuk-radios-item-divider>` is better than matching that, and it costs nothing.

So don't add a `[NonStandardParameter]` HTML sibling next to a text property just to hold inner
content. One `TemplateString?` does the job:

```csharp
public record RadiosOptionsItem
{
    public TemplateString? Divider { get; set; }   // set from <govuk-radios-item-divider>
}
```

### 2. Otherwise, look at how govuk-frontend renders it

For a parameter that *isn't* fed by inner content — one that comes from a tag helper attribute, or
that only `IComponentGenerator` callers set — find it in
`lib/govuk-frontend-<version>/dist/govuk/components/<component>/template.njk`:

- `{{ params.thing }}` — Nunjucks autoescapes `{{ }}`, so the parameter is **text**. Use `string?`.
- `{{ params.thingHtml | safe }}`, or a value passed to `govukSomething({ html: ... })` — **markup**.
  Use `IHtmlContent?`.

`TabsOptions.Title` is the text case: it's only ever set from the `title` attribute, which binds to a
`string?` tag helper property, so markup can't reach it.

### 3. Is it an attribute value?

Attribute values — `Classes`, `Id`, `Href`, `Name`, `Value`, `DescribedBy`, `Type`, `AutoComplete`,
`Pattern` and friends — are the case that genuinely needs the union, and they should stay
`TemplateString?`.

They're populated from `AttributeCollection`, which holds whatever Razor put in
`TagHelperAttribute.Value`:

```csharp
var attributes = new AttributeCollection(output.Attributes);
attributes.Remove("class", out var classes);   // → TemplateString
```

For `class="govuk-tag @extra"` that value is an **already-encoded** `IHtmlContent`. But library code
also assigns plain literals — `Classes = "govuk-tag"` — which are unencoded. The same property has to
accept both, and `TemplateString` is exactly that: a union that records which it holds and encodes
accordingly when written.

Typing these as `string?` would encode Razor's value a second time. Typing them as `IHtmlContent?`
would force every literal to be wrapped, and would let markup into an attribute.

## Things that will bite you

**`AppendHtml` has a `string` overload that does no encoding.** Once a property is `string?`,
`builder.AppendHtml(options.SomeText)` silently binds to it and emits the text as markup. This is
invisible at the call site, so `IHtmlContentBuilder.AppendHtml(string)`,
`HtmlContentBuilder.AppendHtml(string)` and `TagHelperContent.AppendHtml(string)` are banned in this
project — see `src/GovUk.Frontend.AspNetCore/BannedSymbols.txt`. Use `Append` for text, or pass an
`IHtmlContent`.

**`HtmlContentExtensions.ToHtmlString` is banned too.** It returns encoded HTML, so passing the
result somewhere that encodes gives double-encoded output. Use `TemplateString.TryGetText` when you
want a value's text — for a dictionary key, an attribute token or something to parse — and
`TemplateString.Render` when you want the markup.

**A type test against `TemplateString` won't match an `IHtmlContent`.** Content snapshotted from a
tag helper is an `HtmlString`, so `if (context.Value is TemplateString x)` silently stops matching
when a context property is retyped. Use `is { } x`.

**Snapshot content you hold onto.** Razor reuses `TagHelperContent` after a tag helper has rendered,
so anything kept beyond that point needs `content.Snapshot()`.

## Checking your work

The conformance fixtures are the strongest signal: they're govuk-frontend's own, and they assert the
exact rendered output including which attributes appear. If a typing change is correct, they pass
untouched. If they fail, the generator is emitting something upstream doesn't.

Worth adding alongside a new parameter: a test that text content containing `&` is encoded, and — for
an `IHtmlContent?` or a `TemplateString?` holding markup — that it renders as an element rather than
as escaped text. `ComponentGeneration/EncodingTests.cs` has examples of both.
