# Character count validation

The [character count component](../components/character-count.md) only tells users when they've gone over the limit; it doesn't stop them submitting. To validate the limit on the server, counting the text the same way as the component's JavaScript, use the validation attribute that matches how the component's limit is set:

| Component | Validation attribute |
| --- | --- |
| `max-length="200"` | `[CharacterCount(200)]` |
| `max-length="200" count-type="Length"` | `[CharacterCount(200)]` |
| `max-length="200" count-type="Characters"` | `[CharacterCount(200, CountType = CharacterCountType.Characters)]` |
| `max-length="150" count-type="Words"` | `[CharacterCount(150, CountType = CharacterCountType.Words)]` |
| `max-words="150"` (deprecated) | `[MaxWords(150)]` |

The attributes are in the `GovUk.Frontend.AspNetCore.Validation` namespace.

A `null` value is considered valid; use `[Required]` as well if a value must be entered.

### Example

```cs
using GovUk.Frontend.AspNetCore;
using GovUk.Frontend.AspNetCore.Validation;

public class MyModel
{
    [CharacterCount(150, CountType = CharacterCountType.Words, ErrorMessage = "Job description must be 150 words or fewer")]
    public string? JobDescription { get; set; }
}
```

```razor
<govuk-character-count for="JobDescription" max-length="150" count-type="Words">
    <label>Job description</label>
</govuk-character-count>
```

### How text is counted

- **`Length`** counts as JavaScript's `String.length` does, in UTF-16 code units, so some emoji count as two or more. A line break counts as one, as it does in the browser, even though it's submitted as two characters (`\r\n`).
- **`Characters`** counts the characters a user would perceive (grapheme clusters), so an emoji or a letter with a combining accent counts as one.
- **`Words`** counts words as the browser's `Intl.Segmenter` finds them. Punctuation and emoji on their own aren't words, punctuation within a word (as in "don't" or "3.14") doesn't split it, and a hyphen does, so "well-known" is two words.
  Where browsers differ, the validator counts as the browser that finds the fewest words, so it never rejects text a browser showed as within the limit, though it may accept text a browser showed as over it:
  - recent versions of Chrome and Edge split words at a full stop or colon between letters, so "e.g." is two words there, but one in other browsers and to the validator
  - browsers find the words in languages written without spaces between them, such as Chinese, Japanese and Thai, using a dictionary; the validator doesn't, and counts a run of such text as one word
- **`max-words`** counts runs of characters separated by whitespace, so "well-known" is one word and a lone "-" is a word.

## Usage with alternative validation frameworks

If you're not using data annotation attributes for validation, use the helper classes the attributes are built on: `GovUk.Frontend.AspNetCore.Validation.CharacterCountValidator` and, for `max-words`, `GovUk.Frontend.AspNetCore.Validation.MaxWordsValidator`.

`CharacterCountValidator.GetCount` returns the count itself, should you want it for an error message.

### Example with Fluent Validation

#### Custom validator
```cs
using FluentValidation;
using GovUk.Frontend.AspNetCore;
using GovUk.Frontend.AspNetCore.Validation;

public static class RuleBuilderExtensions
{
    public static IRuleBuilderOptions<T, string?> CharacterCount<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        int maxLength,
        CharacterCountType countType = CharacterCountType.Length)
    {
        var validator = new CharacterCountValidator(maxLength, countType);
        return ruleBuilder.Must(value => validator.IsValid(value));
    }
}
```

#### Usage
```cs
public class MyModel
{
    public string? JobDescription { get; set; }
}

public class MyModelValidator : AbstractValidator<MyModel>
{
    public MyModelValidator()
    {
        RuleFor(m => m.JobDescription)
            .CharacterCount(150, CharacterCountType.Words)
            .WithMessage("Job description must be 150 words or fewer");
    }
}
```
