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

Prefer `string?` or `IHtmlContent?`. Reach for `TemplateString?` only when a value genuinely has to
accept both, which in practice means attribute values (see below).

## Deciding which one

### 1. Look at how govuk-frontend renders it

Find the parameter in `lib/govuk-frontend-<version>/dist/govuk/components/<component>/template.njk`.

- `{{ params.thing }}` — Nunjucks **autoescapes** `{{ }}`, so the parameter is **text**. Use `string?`.
- `{{ params.thingHtml | safe }}`, or the value passed to `govukSomething({ html: ... })` — **markup**.
  Use `IHtmlContent?`.

This is worth checking rather than guessing from the name. `title`, `divider` and `number` all sound
like they might carry markup — and in this library three of them are settable from Razor child
content — but upstream renders every one of them with `{{ }}`, so upstream's parameter is text.

### 2. Does a tag helper capture child content for it?

Razor child content is always markup. If a tag helper writes to the parameter from
`output.GetChildContentAsync()`, the options record needs somewhere markup can go.

Where upstream has a `text` parameter but this library also lets you write content inside the
element, add an HTML sibling rather than widening the text one:

```csharp
public record RadiosOptionsItem
{
    public string? Divider { get; set; }
    [NonStandardParameter]
    public IHtmlContent? DividerHtml { get; set; }
}
```

`[NonStandardParameter]` marks a parameter that isn't in the reference Nunjucks implementation. The
generator then picks between them with `HtmlOrText(item.DividerHtml, item.Divider)`.

`SelectOptionsItem.Html`, `RadiosOptionsItem.DividerHtml`, `CheckboxesOptionsItem.DividerHtml` and
`PaginationOptionsItem.NumberHtml` all exist for this reason.

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

Worth adding alongside a new parameter: a test that text content containing `&` is encoded, and that
an HTML sibling renders as an element. `DefaultComponentGeneratorTests.NonStandardParameters.cs` and
`ComponentGeneration/EncodingTests.cs` have examples.
