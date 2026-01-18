using System.Globalization;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual Task<GovUkComponent> GenerateTableAsync(TableOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var tableTag = new HtmlTag("table", attrs => attrs
            .WithClasses("govuk-table", options.Classes)
            .With(options.Attributes));

        if (options.Caption is not null)
        {
            var captionTag = new HtmlTag("caption", attrs => attrs
                .WithClasses("govuk-table__caption", options.CaptionClasses));
            captionTag.InnerHtml.AppendHtml(options.Caption);
            tableTag.InnerHtml.AppendHtml(captionTag);
        }

        if (options.Head is not null)
        {
            var theadTag = GenerateTableHead(options.Head);
            tableTag.InnerHtml.AppendHtml(theadTag);
        }

        var tbodyTag = GenerateTableBody(options.Rows, options.FirstCellIsHeader);
        tableTag.InnerHtml.AppendHtml(tbodyTag);

        return GenerateFromHtmlTagAsync(tableTag);

        HtmlTag GenerateTableHead(IReadOnlyCollection<TableOptionsHead> head)
        {
            var theadTag = new HtmlTag("thead", attrs => attrs
                .WithClasses("govuk-table__head"));

            var trTag = new HtmlTag("tr", attrs => attrs
                .WithClasses("govuk-table__row"));

            foreach (var item in head)
            {
                var thTag = new HtmlTag("th", attrs =>
                {
                    attrs
                        .With("scope", "col")
                        .WithClasses(
                            "govuk-table__header",
                            item.Format is not null ? $"govuk-table__header--{item.Format.ToHtmlString()}" : null,
                            item.Classes);

                    if (item.ColSpan.HasValue)
                    {
                        attrs.With("colspan", item.ColSpan.Value.ToString(CultureInfo.InvariantCulture));
                    }

                    if (item.RowSpan.HasValue)
                    {
                        attrs.With("rowspan", item.RowSpan.Value.ToString(CultureInfo.InvariantCulture));
                    }

                    attrs.With(item.Attributes);
                });

                thTag.InnerHtml.AppendHtml(HtmlOrText(item.Html, item.Text));
                trTag.InnerHtml.AppendHtml(thTag);
            }

            theadTag.InnerHtml.AppendHtml(trTag);
            return theadTag;
        }

        HtmlTag GenerateTableBody(IReadOnlyCollection<IReadOnlyCollection<TableOptionsColumn>>? rows, bool? firstCellIsHeader)
        {
            var tbodyTag = new HtmlTag("tbody", attrs => attrs
                .WithClasses("govuk-table__body"));

            if (rows is not null)
            {
                foreach (var row in rows)
                {
                    if (row is not null)
                    {
                        var trTag = new HtmlTag("tr", attrs => attrs
                            .WithClasses("govuk-table__row"));

                        var isFirstCell = true;
                        foreach (var cell in row)
                        {
                            if (isFirstCell && firstCellIsHeader == true)
                            {
                                var thTag = CreateHeaderCell(cell);
                                trTag.InnerHtml.AppendHtml(thTag);
                            }
                            else
                            {
                                var tdTag = CreateDataCell(cell);
                                trTag.InnerHtml.AppendHtml(tdTag);
                            }

                            isFirstCell = false;
                        }

                        tbodyTag.InnerHtml.AppendHtml(trTag);
                    }
                }
            }

            return tbodyTag;
        }

        HtmlTag CreateHeaderCell(TableOptionsColumn cell)
        {
            var thTag = new HtmlTag("th", attrs =>
            {
                attrs
                    .With("scope", "row")
                    .WithClasses("govuk-table__header", cell.Classes);

                AddCellSpanAttributes(attrs, cell);
            });

            thTag.InnerHtml.AppendHtml(HtmlOrText(cell.Html, cell.Text));
            return thTag;
        }

        HtmlTag CreateDataCell(TableOptionsColumn cell)
        {
            var tdTag = new HtmlTag("td", attrs =>
            {
                attrs.WithClasses(
                    "govuk-table__cell",
                    cell.Format is not null ? $"govuk-table__cell--{cell.Format.ToHtmlString()}" : null,
                    cell.Classes);

                AddCellSpanAttributes(attrs, cell);
            });

            tdTag.InnerHtml.AppendHtml(HtmlOrText(cell.Html, cell.Text));
            return tdTag;
        }

        void AddCellSpanAttributes(HtmlTag.AttributeBuilder attrs, TableOptionsColumn cell)
        {
            if (cell.ColSpan.HasValue)
            {
                attrs.With("colspan", cell.ColSpan.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (cell.RowSpan.HasValue)
            {
                attrs.With("rowspan", cell.RowSpan.Value.ToString(CultureInfo.InvariantCulture));
            }

            attrs.With(cell.Attributes);
        }
    }
}
