using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.HtmlGeneration;

internal partial class ComponentGenerator
{
    internal const string CheckboxesElement = "div";
    internal const string CheckboxesDividerItemElement = "div";
    internal const string CheckboxesItemElement = "div";
    internal const CheckboxesItemBehavior CheckboxesItemDefaultBehavior = CheckboxesItemBehavior.Default;
    internal const bool CheckboxesItemDefaultChecked = false;
    internal const bool CheckboxesItemDefaultDisabled = false;

    public TagBuilder GenerateCheckboxes(
        string? idPrefix,
        string? name,
        string? describedBy,
        bool hasFieldset,
        IEnumerable<CheckboxesItemBase> items,
        AttributeDictionary? attributes)
    {
        Guard.ArgumentNotNull(nameof(items), items);

        var isConditional = items.OfType<CheckboxesItem>().Any(i => i.Conditional is not null);

        var tagBuilder = new TagBuilder(CheckboxesElement);
        tagBuilder.MergeOptionalAttributes(attributes);
        tagBuilder.MergeCssClass("govuk-checkboxes");
        tagBuilder.Attributes.Add("data-module", "govuk-checkboxes");

        var itemIndex = 0;
        foreach (var item in items)
        {
            if (item is CheckboxesItem checkboxesItem)
            {
                AddItem(checkboxesItem);
            }
            else if (item is CheckboxesItemDivider divider)
            {
                AddDivider(divider);
            }
            else
            {
                throw new NotSupportedException($"Unknown item type: '{item.GetType().FullName}'.");
            }

            itemIndex++;
        }

        return tagBuilder;

        void AddDivider(CheckboxesItemDivider divider)
        {
            Guard.ArgumentValidNotNull(
                nameof(items),
                $"Item {itemIndex} is not valid; {nameof(CheckboxesItemDivider.Content)} cannot be null.",
                divider.Content,
                divider.Content is not null);

            var dividerTagBuilder = new TagBuilder(CheckboxesDividerItemElement);
            dividerTagBuilder.MergeOptionalAttributes(divider.Attributes);
            dividerTagBuilder.MergeCssClass("govuk-checkboxes__divider");
            dividerTagBuilder.InnerHtml.AppendHtml(divider.Content);

            tagBuilder.InnerHtml.AppendHtml(dividerTagBuilder);
        }

        void AddItem(CheckboxesItem item)
        {
            Guard.ArgumentValidNotNull(
                nameof(items),
                $"Item {itemIndex} is not valid; {nameof(CheckboxesItem.Value)} cannot be null.",
                item.Value,
                item.Value is not null);

            Guard.ArgumentValid(
                nameof(items),
                $"Item {itemIndex} is not valid; {nameof(CheckboxesItem.Name)} cannot be null when {nameof(name)} is null.",
                item.Name is not null || name is not null);

            Guard.ArgumentValid(
                nameof(items),
                $"Item {itemIndex} is not valid; {nameof(CheckboxesItem.Id)} cannot be null when {nameof(idPrefix)} is null.",
                item.Id is not null || idPrefix is not null);

            var itemId = item.Id ?? (itemIndex == 0 ? idPrefix : $"{idPrefix}-{itemIndex + 1}");
            var hintId = itemId + "-item-hint";
            var conditionalId = "conditional-" + itemId;

            var itemName = item.Name ?? name;

            var itemTagBuilder = new TagBuilder(CheckboxesItemElement);
            itemTagBuilder.MergeOptionalAttributes(item.Attributes);
            itemTagBuilder.MergeCssClass("govuk-checkboxes__item");

            var input = new TagBuilder("input");
            input.MergeOptionalAttributes(item.InputAttributes);
            input.TagRenderMode = TagRenderMode.SelfClosing;
            input.MergeCssClass("govuk-checkboxes__input");
            input.Attributes.Add("id", itemId);
            input.Attributes.Add("name", itemName);
            input.Attributes.Add("type", "checkbox");
            input.Attributes.Add("value", item.Value);

            if (item.Checked)
            {
                input.Attributes.Add("checked", "checked");
            }

            if (item.Disabled)
            {
                input.Attributes.Add("disabled", "disabled");
            }

            if (item.Conditional is not null)
            {
                input.Attributes.Add("data-aria-controls", conditionalId);
            }

            if (item.Behavior == CheckboxesItemBehavior.Exclusive)
            {
                input.Attributes.Add("data-behaviour", "exclusive");
            }

            var itemDescribedBy = !hasFieldset ? describedBy : null;

            if (item.Hint is not null)
            {
                AppendToDescribedBy(ref itemDescribedBy, hintId);
            }

            if (!string.IsNullOrEmpty(itemDescribedBy))
            {
                input.Attributes.Add("aria-describedby", itemDescribedBy);
            }

            itemTagBuilder.InnerHtml.AppendHtml(input);

            var label = GenerateLabel(itemId, isPageHeading: false, content: item.LabelContent, item.LabelAttributes);
            if (label is not null)
            {
                label.MergeCssClass("govuk-checkboxes__label");
                itemTagBuilder.InnerHtml.AppendHtml(label);
            }

            if (item.Hint is not null)
            {
                Guard.ArgumentValidNotNull(
                    nameof(items),
                    $"Item {itemIndex} is not valid; {nameof(CheckboxesItem.Hint)}.{nameof(CheckboxesItemHint.Content)} cannot be null.",
                    item.Hint.Content,
                    item.Hint.Content is not null);

                var hint = GenerateHint(hintId, item.Hint.Content, item.Hint.Attributes);
                hint.MergeCssClass("govuk-checkboxes__hint");
                itemTagBuilder.InnerHtml.AppendHtml(hint);
            }

            tagBuilder.InnerHtml.AppendHtml(itemTagBuilder);

            if (item.Conditional is not null)
            {
                Guard.ArgumentValidNotNull(
                    nameof(items),
                    $"Item {itemIndex} is not valid; {nameof(CheckboxesItem.Conditional.Content)} cannot be null.",
                    item.Conditional.Content,
                    item.Conditional.Content is not null);

                var conditional = new TagBuilder("div");
                conditional.MergeOptionalAttributes(item.Conditional.Attributes);
                conditional.MergeCssClass("govuk-checkboxes__conditional");

                if (!item.Checked)
                {
                    conditional.MergeCssClass("govuk-checkboxes__conditional--hidden");
                }

                conditional.Attributes.Add("id", conditionalId);

                conditional.InnerHtml.AppendHtml(item.Conditional.Content);

                tagBuilder.InnerHtml.AppendHtml(conditional);
            }
        }
    }
}
