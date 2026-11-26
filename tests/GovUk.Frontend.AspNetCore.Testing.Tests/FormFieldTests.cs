using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.Testing.Tests;

public class FormFieldTests
{
    [Fact]
    public async Task TextInput_ReadsLabelHintErrorValueAndAffixes()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateInputAsync(new InputOptions
        {
            Id = "cost",
            Name = "Cost",
            Value = "45",
            Label = new LabelOptions { Text = "Cost per item" },
            Hint = new HintOptions { Text = "Enter the cost in pounds" },
            ErrorMessage = new ErrorMessageOptions { Text = "Enter a cost" },
            Prefix = new InputOptionsPrefix { Text = "£" },
            Suffix = new InputOptionsSuffix { Text = "per item" }
        }));

        var input = doc.GetTextInputByLabel("Cost per item");

        Assert.Equal("Cost per item", input.Label);
        Assert.Equal("Enter the cost in pounds", input.Hint);
        Assert.Equal("Enter a cost", input.ErrorMessage);
        Assert.Equal("45", input.Value);
        Assert.Equal("Cost", input.Name);
        Assert.Equal("text", input.Type);
        Assert.Equal("£", input.Prefix);
        Assert.Equal("per item", input.Suffix);
        Assert.Equal("cost", input.Input.Id);
        Assert.Contains("govuk-form-group", input.Element.ClassList);
        Assert.Equal("label", input.LabelElement!.LocalName);
    }

    [Fact]
    public async Task TextInput_WithoutHintOrError_HasNulls()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateInputAsync(new InputOptions
        {
            Id = "name",
            Name = "Name",
            Label = new LabelOptions { Text = "Full name", IsPageHeading = true }
        }));

        var input = doc.GetTextInputByLabel("Full name");

        Assert.Null(input.Hint);
        Assert.Null(input.HintElement);
        Assert.Null(input.ErrorMessage);
        Assert.Null(input.ErrorMessageElement);
        Assert.Equal("", input.Value);
        Assert.Null(input.Prefix);
        Assert.Null(input.Suffix);
    }

    [Fact]
    public async Task ErrorMessage_LeavesOutVisuallyHiddenPrefix()
    {
        var doc = await Render.DocumentAsync(
            g => g.GenerateInputAsync(new InputOptions
            {
                Id = "email",
                Name = "Email",
                Label = new LabelOptions { Text = "Email address" },
                ErrorMessage = new ErrorMessageOptions { Text = "Enter an email address" }
            }),
            g => g.GenerateInputAsync(new InputOptions
            {
                Id = "phone",
                Name = "Phone",
                Label = new LabelOptions { Text = "Phone" },
                ErrorMessage = new ErrorMessageOptions { Text = "Enter a phone number", VisuallyHiddenText = "Gwall" }
            }));

        var email = doc.GetTextInputByLabel("Email address");
        Assert.Equal("Enter an email address", email.ErrorMessage);
        Assert.Contains("Error:", email.ErrorMessageElement!.TextContent, StringComparison.Ordinal);

        Assert.Equal("Enter a phone number", doc.GetTextInputByLabel("Phone").ErrorMessage);
    }

    [Fact]
    public async Task Textarea_ReadsValue()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateTextareaAsync(new TextareaOptions
        {
            Id = "more-detail",
            Name = "MoreDetail",
            Value = "Line one\nLine two",
            Label = new LabelOptions { Text = "Can you provide more detail?" },
            Hint = new HintOptions { Text = "Do not include personal information" }
        }));

        var textarea = doc.GetTextareaByLabel("Can you provide more detail?");

        Assert.IsNotType<CharacterCount>(textarea);
        Assert.Equal("Line one\nLine two", textarea.Value);
        Assert.Equal("MoreDetail", textarea.Name);
        Assert.Equal("Do not include personal information", textarea.Hint);
    }

    [Fact]
    public async Task CharacterCount_ReadsLimitAndIgnoresCountMessage()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateCharacterCountAsync(new CharacterCountOptions
        {
            Id = "job-description",
            Name = "JobDescription",
            Value = "Teacher",
            MaxLength = 200,
            Threshold = 75,
            Label = new LabelOptions { Text = "Describe your job" },
            ErrorMessage = new ErrorMessageOptions { Text = "Job description must be 200 characters or fewer" }
        }));

        var count = doc.GetCharacterCountByLabel("Describe your job");

        Assert.Equal(200, count.MaxLength);
        Assert.Null(count.MaxWords);
        Assert.Equal(75, count.Threshold);
        Assert.Equal("Teacher", count.Value);
        Assert.Null(count.Hint);
        Assert.Equal("Job description must be 200 characters or fewer", count.ErrorMessage);

        // A character count is a textarea too.
        Assert.Same(count.TextareaElement, doc.GetTextareaByLabel("Describe your job").TextareaElement);
        Assert.IsType<CharacterCount>(Assert.Single(doc.Fields));
    }

    [Fact]
    public async Task CharacterCount_WithMaxWordsAndHint()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateCharacterCountAsync(new CharacterCountOptions
        {
            Id = "summary",
            Name = "Summary",
            MaxWords = 150,
            Label = new LabelOptions { Text = "Summary" },
            Hint = new HintOptions { Text = "Keep it short" }
        }));

        var count = doc.GetCharacterCountByLabel("Summary");

        Assert.Null(count.MaxLength);
        Assert.Equal(150, count.MaxWords);
        Assert.Equal("Keep it short", count.Hint);
    }

    [Fact]
    public async Task Select_ReadsItemsAndSelectedItem()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateSelectInputAsync(new SelectOptions
        {
            Id = "sort",
            Name = "Sort",
            Label = new LabelOptions { Text = "Sort by" },
            Items =
            [
                new SelectOptionsItem { Value = "published", Text = "Recently published" },
                new SelectOptionsItem { Value = "updated", Text = "Recently updated", Selected = true },
                new SelectOptionsItem { Value = "views", Text = "Most views", Disabled = true }
            ]
        }));

        var select = doc.GetSelectByLabel("Sort by");

        Assert.Equal(["Recently published", "Recently updated", "Most views"], select.Items.Select(i => i.Text));
        Assert.Equal(["published", "updated", "views"], select.Items.Select(i => i.Value));
        Assert.Equal("Recently updated", select.SelectedItem!.Text);
        Assert.Equal("updated", select.Value);
        Assert.Equal([false, true, false], select.Items.Select(i => i.IsSelected));
        Assert.True(select.GetItemByText("Most views").IsDisabled);
        Assert.Null(select.FindItemByText("Most comments"));
        Assert.Equal("Sort", select.Name);
    }

    [Fact]
    public async Task Select_NothingSelected_ValueIsFirstEnabledItem()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateSelectInputAsync(new SelectOptions
        {
            Id = "country",
            Name = "Country",
            Label = new LabelOptions { Text = "Country" },
            Items =
            [
                new SelectOptionsItem { Value = "", Text = "Choose a country", Disabled = true },
                new SelectOptionsItem { Value = "england", Text = "England" },
                new SelectOptionsItem { Value = "wales", Text = "Wales" }
            ]
        }));

        var select = doc.GetSelectByLabel("Country");

        Assert.Equal([false, true, false], select.Items.Select(i => i.IsSelected));
        Assert.Equal("England", select.SelectedItem!.Text);
        Assert.Equal("england", select.Value);
    }

    [Fact]
    public async Task Select_GetItem_NotFound_ThrowsWithOptionText()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateSelectInputAsync(new SelectOptions
        {
            Id = "country",
            Name = "Country",
            Label = new LabelOptions { Text = "Country" },
            Items = [new SelectOptionsItem { Value = "england", Text = "England" }, new SelectOptionsItem { Value = "wales", Text = "Wales" }]
        }));

        var ex = Assert.Throws<ComponentLookupException>(() => doc.GetSelectByLabel("Country").GetItemByText("Scotland"));

        Assert.Equal("Select option not found.", ex.Failure.Summary);
        Assert.Equal("an option with the text \"Scotland\"", ex.Failure.Expected);
        Assert.Equal("options with the text \"England\", \"Wales\"", ex.Failure.Found);
    }

    [Fact]
    public async Task PasswordInput_ReadsValue()
    {
        var doc = await Render.DocumentAsync(g => g.GeneratePasswordInputAsync(new PasswordInputOptions
        {
            Id = "password",
            Name = "Password",
            Value = "hunter2",
            Label = new LabelOptions { Text = "Password" },
            Hint = new HintOptions { Text = "At least 8 characters" }
        }));

        var password = doc.GetPasswordInputByLabel("Password");

        Assert.Equal("hunter2", password.Value);
        Assert.Equal("Password", password.Name);
        Assert.Equal("At least 8 characters", password.Hint);

        // A password input isn't a text input.
        Assert.Null(doc.FindTextInputByLabel("Password"));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task FileUpload_ReadsAttributes(bool javaScript)
    {
        var doc = await Render.DocumentAsync(g => g.GenerateFileUploadAsync(new FileUploadOptions
        {
            Id = "file",
            Name = "File",
            Multiple = true,
            JavaScript = javaScript,
            Label = new LabelOptions { Text = "Upload a file" },
            ErrorMessage = new ErrorMessageOptions { Text = "Select a file" },
            Attributes = new AttributeCollection { { "accept", ".pdf" } }
        }));

        var upload = doc.GetFileUploadByLabel("Upload a file");

        Assert.True(upload.AllowsMultiple);
        Assert.Equal(".pdf", upload.Accept);
        Assert.Equal("File", upload.Name);
        Assert.Equal("Select a file", upload.ErrorMessage);
    }

    [Fact]
    public async Task DateInput_ReadsLegendPartsAndValue()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateDateInputAsync(new DateInputOptions
        {
            Id = "dob",
            NamePrefix = "dob",
            Fieldset = new FieldsetOptions { Legend = new FieldsetOptionsLegend { Text = "What is your date of birth?", IsPageHeading = true } },
            Hint = new HintOptions { Text = "For example, 27 3 2007" },
            ErrorMessage = new ErrorMessageOptions { Text = "Date of birth must include a year" },
            Items =
            [
                new DateInputOptionsItem { Name = "day", Value = "27" },
                new DateInputOptionsItem { Name = "month", Value = "3" },
                new DateInputOptionsItem { Name = "year", Value = "", Error = true }
            ]
        }));

        var date = doc.GetDateInputByLegend("What is your date of birth?");

        Assert.Equal("legend", date.LabelElement!.LocalName);
        Assert.Equal("For example, 27 3 2007", date.Hint);
        Assert.Equal("Date of birth must include a year", date.ErrorMessage);
        Assert.Equal(["Day", "Month", "Year"], date.Items.Select(i => i.Label));
        Assert.Equal("dob-day", date.Day!.Name);
        Assert.Equal("27", date.Day.Value);
        Assert.Equal("3", date.Month!.Value);
        Assert.Equal("", date.Year!.Value);
        Assert.False(date.Day.HasError);
        Assert.True(date.Year.HasError);
        Assert.Null(date.Value);

        // The day, month and year inputs aren't fields in their own right.
        Assert.Same(date.Element, Assert.IsType<DateInput>(Assert.Single(doc.Fields)).Element);
    }

    [Theory]
    [InlineData("27", "3", "2007", 2007, 3, 27)]
    [InlineData("01", "02", "2024", 2024, 2, 1)]
    [InlineData(" 29 ", "2", "2024", 2024, 2, 29)]
    [InlineData("27", "Mar", "2007", 2007, 3, 27)]
    [InlineData("27", "march", "2007", 2007, 3, 27)]
    [InlineData("1", " SEPTEMBER ", "2024", 2024, 9, 1)]
    [InlineData("31", "May", "2024", 2024, 5, 31)]
    public async Task DateInput_Value_RealDate(string day, string month, string year, int expectedYear, int expectedMonth, int expectedDay)
    {
        var date = await RenderDateAsync(day, month, year);

        Assert.Equal(new DateOnly(expectedYear, expectedMonth, expectedDay), date.Value);
    }

    [Theory]
    [InlineData("29", "2", "2023")]
    [InlineData("31", "4", "2024")]
    [InlineData("x", "1", "2024")]
    [InlineData("1", "13", "2024")]
    [InlineData("1", "Marc", "2024")]
    [InlineData("1", "Sept", "2024")]
    [InlineData("30", "Feb", "2024")]
    [InlineData("1", "1", "")]
    [InlineData("-1", "1", "2024")]
    [InlineData("1", "1", "0")]
    public async Task DateInput_Value_NotARealDate_IsNull(string day, string month, string year)
    {
        var date = await RenderDateAsync(day, month, year);

        Assert.Null(date.Value);
    }

    [Fact]
    public async Task DateInput_MonthAndYearOnly_HasNoDayAndNoValue()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateDateInputAsync(new DateInputOptions
        {
            Id = "issued",
            Fieldset = new FieldsetOptions { Legend = new FieldsetOptionsLegend { Text = "When was your passport issued?" } },
            Items =
            [
                new DateInputOptionsItem { Name = "Issued.Month", Label = new HtmlString("Month"), Value = "3" },
                new DateInputOptionsItem { Name = "Issued.Year", Label = new HtmlString("Year"), Value = "2020" }
            ]
        }));

        var date = doc.GetDateInputByLegend("When was your passport issued?");

        Assert.Null(date.Day);
        Assert.Equal("3", date.Month!.Value);
        Assert.Equal("2020", date.Year!.Value);
        Assert.Null(date.Value);
    }

    [Fact]
    public async Task DateInput_PartFromLabel_WhenNameAndIdDoNotSayWhichPart()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateDateInputAsync(new DateInputOptions
        {
            Id = "start",
            Fieldset = new FieldsetOptions { Legend = new FieldsetOptionsLegend { Text = "Start date" } },
            Items =
            [
                new DateInputOptionsItem { Id = "d", Name = "d", Label = new HtmlString("Day"), Value = "1" },
                new DateInputOptionsItem { Id = "m", Name = "m", Label = new HtmlString("Month"), Value = "2" },
                new DateInputOptionsItem { Id = "y", Name = "y", Label = new HtmlString("Year"), Value = "2024" }
            ]
        }));

        Assert.Equal(new DateOnly(2024, 2, 1), doc.GetDateInputByLegend("Start date").Value);
    }

    [Fact]
    public async Task Radios_ReadsItemsCheckedItemAndConditional()
    {
        var conditionalHtml = await InputHtmlAsync("contact-email", "Email address", hint: "For example, name@example.com");

        var doc = await Render.DocumentAsync(g => g.GenerateRadiosAsync(new RadiosOptions
        {
            Name = "Contact",
            IdPrefix = "contact",
            Fieldset = new FieldsetOptions { Legend = new FieldsetOptionsLegend { Text = "How would you like to be contacted?" } },
            Hint = new HintOptions { Text = "Select one option" },
            ErrorMessage = new ErrorMessageOptions { Text = "Select how you would like to be contacted" },
            Items =
            [
                new RadiosOptionsItem
                {
                    Value = "email",
                    Text = "Email",
                    Hint = new HintOptions { Text = "We'll reply within a day" },
                    Checked = true,
                    Conditional = new RadiosOptionsItemConditional { Html = new HtmlString(conditionalHtml) }
                },
                new RadiosOptionsItem { Value = "phone", Text = "Phone" },
                new RadiosOptionsItem { Divider = "or" },
                new RadiosOptionsItem { Value = "none", Text = "Do not contact me", Disabled = true }
            ]
        }));

        var radios = doc.GetRadiosByLegend("How would you like to be contacted?");

        Assert.Equal("Select one option", radios.Hint);
        Assert.Equal("Select how you would like to be contacted", radios.ErrorMessage);
        Assert.Equal(["Email", "Phone", "Do not contact me"], radios.Items.Select(i => i.Label));
        Assert.Equal("email", radios.Value);
        Assert.Equal("Email", radios.CheckedItem!.Label);

        var email = radios.GetItemByLabel("Email");
        Assert.True(email.IsChecked);
        Assert.Equal("We'll reply within a day", email.Hint);
        Assert.Equal("Contact", email.Name);
        Assert.NotNull(email.ConditionalElement);

        // Fields in a conditional reveal can be found from it, and don't leak their hint into the radios'.
        var nested = email.ConditionalElement!.GetTextInputByLabel("Email address");
        Assert.Equal("For example, name@example.com", nested.Hint);
        Assert.Same(nested.Input, doc.GetTextInputByLabel("Email address").Input);

        var phone = radios.GetItemByLabel("Phone");
        Assert.False(phone.IsChecked);
        Assert.Null(phone.Hint);
        Assert.Null(phone.ConditionalElement);

        Assert.True(radios.GetItemByLabel("Do not contact me").IsDisabled);

        Assert.Equal(
            ["How would you like to be contacted?", "Email address"],
            doc.Fields.Select(f => f.Label));
    }

    [Fact]
    public async Task Radios_NoneChecked_ValueIsNull()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateRadiosAsync(new RadiosOptions
        {
            Name = "Colour",
            Fieldset = new FieldsetOptions { Legend = new FieldsetOptionsLegend { Text = "Colour" } },
            Items = [new RadiosOptionsItem { Value = "red", Text = "Red" }, new RadiosOptionsItem { Value = "blue", Text = "Blue" }]
        }));

        var radios = doc.GetRadiosByLegend("Colour");

        Assert.Null(radios.CheckedItem);
        Assert.Null(radios.Value);
    }

    [Fact]
    public async Task Radios_GetItem_NotFound_ThrowsWithLabels()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateRadiosAsync(new RadiosOptions
        {
            Name = "Colour",
            Fieldset = new FieldsetOptions { Legend = new FieldsetOptionsLegend { Text = "Colour" } },
            Items = [new RadiosOptionsItem { Value = "red", Text = "Red" }, new RadiosOptionsItem { Value = "blue", Text = "Blue" }]
        }));

        var ex = Assert.Throws<ComponentLookupException>(() => doc.GetRadiosByLegend("Colour").GetItemByLabel("Green"));

        Assert.Equal("Radio not found.", ex.Failure.Summary);
        Assert.Equal("a radio labelled \"Green\"", ex.Failure.Expected);
        Assert.Equal("radios with the labels \"Red\", \"Blue\"", ex.Failure.Found);
        Assert.StartsWith("<div class=\"govuk-form-group\">", ex.Failure.Html);
        Assert.Null(doc.GetRadiosByLegend("Colour").FindItemByLabel("Green"));
    }

    [Fact]
    public async Task Radios_WithoutFieldset_HasNoLabelButReadsHintAndError()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateRadiosAsync(new RadiosOptions
        {
            Name = "Agree",
            Hint = new HintOptions { Text = "You must agree" },
            ErrorMessage = new ErrorMessageOptions { Text = "Select whether you agree" },
            Items = [new RadiosOptionsItem { Value = "yes", Text = "Yes" }, new RadiosOptionsItem { Value = "no", Text = "No" }]
        }));

        var radios = Assert.IsType<Radios>(Assert.Single(doc.Fields));

        Assert.Null(radios.Label);
        Assert.Null(radios.LabelElement);
        Assert.Equal("You must agree", radios.Hint);
        Assert.Equal("Select whether you agree", radios.ErrorMessage);
        Assert.Equal("Unlabelled radios", radios.ToString());
    }

    [Fact]
    public async Task Checkboxes_ReadsItemsCheckedItemsAndExclusive()
    {
        var conditionalHtml = await InputHtmlAsync("other-country", "Country");

        var doc = await Render.DocumentAsync(g => g.GenerateCheckboxesAsync(new CheckboxesOptions
        {
            Name = "Nationality",
            IdPrefix = "nationality",
            Fieldset = new FieldsetOptions { Legend = new FieldsetOptionsLegend { Text = "What is your nationality?" } },
            Hint = new HintOptions { Text = "Select all that apply" },
            Items =
            [
                new CheckboxesOptionsItem { Value = "british", Text = "British", Checked = true },
                new CheckboxesOptionsItem { Value = "irish", Text = "Irish" },
                new CheckboxesOptionsItem
                {
                    Value = "other",
                    Text = "Citizen of another country",
                    Checked = true,
                    Conditional = new CheckboxesOptionsItemConditional { Html = new HtmlString(conditionalHtml) }
                },
                new CheckboxesOptionsItem { Divider = "or" },
                new CheckboxesOptionsItem { Value = "none", Text = "None of the above", Behaviour = "exclusive" }
            ]
        }));

        var checkboxes = doc.GetCheckboxesByLegend("What is your nationality?");

        Assert.Equal("Select all that apply", checkboxes.Hint);
        Assert.Null(checkboxes.ErrorMessage);
        Assert.Equal(["British", "Irish", "Citizen of another country", "None of the above"], checkboxes.Items.Select(i => i.Label));
        Assert.Equal(["British", "Citizen of another country"], checkboxes.CheckedItems.Select(i => i.Label));
        Assert.Equal(["british", "other"], checkboxes.Values);
        Assert.True(checkboxes.GetItemByLabel("None of the above").IsExclusive);
        Assert.False(checkboxes.GetItemByLabel("British").IsExclusive);
        Assert.Equal("Country", checkboxes.GetItemByLabel("Citizen of another country").ConditionalElement!.Fields.Single().Label);
    }

    [Fact]
    public async Task Values_FollowChangesToTheDocument()
    {
        var doc = await Render.DocumentAsync(
            g => g.GenerateInputAsync(new InputOptions { Id = "name", Name = "Name", Label = new LabelOptions { Text = "Name" } }),
            g => g.GenerateRadiosAsync(new RadiosOptions
            {
                Name = "Colour",
                Fieldset = new FieldsetOptions { Legend = new FieldsetOptionsLegend { Text = "Colour" } },
                Items = [new RadiosOptionsItem { Value = "red", Text = "Red" }, new RadiosOptionsItem { Value = "blue", Text = "Blue" }]
            }));

        var name = doc.GetTextInputByLabel("Name");
        var colour = doc.GetRadiosByLegend("Colour");

        name.Input.Value = "Sarah";
        colour.GetItemByLabel("Blue").Input.IsChecked = true;

        Assert.Equal("Sarah", name.Value);
        Assert.Equal("blue", colour.Value);
    }

    [Fact]
    public async Task GetFieldByLabelOrLegend_FindsAnyKindByLabelOrLegend()
    {
        var doc = await RenderFormAsync();

        Assert.IsType<TextInput>(doc.GetFieldByLabelOrLegend("Full name"));
        Assert.IsType<DateInput>(doc.GetFieldByLabelOrLegend("Date of birth"));
        Assert.IsType<Radios>(doc.GetFieldByLabelOrLegend("Colour"));
        Assert.Null(doc.FindFieldByLabelOrLegend("Email address"));
        Assert.Equal(["Full name", "Date of birth", "Colour"], doc.Fields.Select(f => f.Label));
        Assert.Equal(["Text input: Full name", "Date input: Date of birth", "Radios: Colour"], doc.Fields.Select(f => f.ToString()));
    }

    [Fact]
    public async Task GetFieldByLabelOrLegend_NotFound_ListsLabelsAndLegendsOnThePage()
    {
        var doc = await RenderFormAsync();

        var ex = Assert.Throws<ComponentLookupException>(() => doc.GetFieldByLabelOrLegend("Email address"));

        Assert.Equal(
            """
            Field not found.
            Expected: a field with the label or legend "Email address"
            Found:    fields labelled "Full name" (text input), "Date of birth" (date input), "Colour" (radios)
            """,
            ex.Message);
        Assert.Null(ex.Failure.Html);
    }

    [Fact]
    public async Task GetTextInputByLabel_WrongKind_ListsKindsOfFields()
    {
        var doc = await RenderFormAsync();

        var ex = Assert.Throws<ComponentLookupException>(() => doc.GetTextInputByLabel("Colour"));

        Assert.Equal("Text input not found.", ex.Failure.Summary);
        Assert.Equal("a text input labelled \"Colour\"", ex.Failure.Expected);
        Assert.Equal("fields labelled \"Full name\" (text input), \"Date of birth\" (date input), \"Colour\" (radios)", ex.Failure.Found);
    }

    [Fact]
    public async Task GetRadiosByLegend_NotFound_UsesRadiosWording()
    {
        var doc = await RenderFormAsync();

        var ex = Assert.Throws<ComponentLookupException>(() => doc.GetRadiosByLegend("Size"));

        Assert.Equal("Radios not found.", ex.Failure.Summary);
        Assert.Equal("radios with the legend \"Size\"", ex.Failure.Expected);
    }

    [Fact]
    public void GetFieldByLabelOrLegend_NoFields_FoundIsNoFields()
    {
        var doc = Render.Document("<p>Nothing here</p>");

        var ex = Assert.Throws<ComponentLookupException>(() => doc.GetFieldByLabelOrLegend("Name"));

        Assert.Equal("no fields", ex.Failure.Found);
    }

    [Fact]
    public async Task GetFieldByLabelOrLegend_Unlabelled_IsListedAsUnlabelled()
    {
        var doc = await Render.DocumentAsync(
            g => g.GenerateInputAsync(new InputOptions { Id = "name", Name = "Name", Label = new LabelOptions { Text = "Name" } }),
            g => g.GenerateCheckboxesAsync(new CheckboxesOptions
            {
                Name = "Terms",
                Items = [new CheckboxesOptionsItem { Value = "yes", Text = "I agree to the terms" }]
            }));

        var ex = Assert.Throws<ComponentLookupException>(() => doc.GetCheckboxesByLegend("I agree to the terms"));

        Assert.Equal("fields labelled \"Name\" (text input), unlabelled checkboxes", ex.Failure.Found);
    }

    [Fact]
    public async Task GetTextInputByLabel_Duplicate_Throws()
    {
        var doc = await Render.DocumentAsync(
            g => g.GenerateInputAsync(new InputOptions { Id = "a", Name = "A", Label = new LabelOptions { Text = "Name" } }),
            g => g.GenerateInputAsync(new InputOptions { Id = "b", Name = "B", Label = new LabelOptions { Text = "Name" } }));

        var ex = Assert.Throws<ComponentLookupException>(() => doc.GetTextInputByLabel("Name"));
        Assert.Equal("More than one text input found.", ex.Failure.Summary);
        Assert.Equal("2 text inputs labelled \"Name\"", ex.Failure.Found);

        Assert.Throws<ComponentLookupException>(() => doc.FindTextInputByLabel("Name"));
    }

    [Fact]
    public async Task GetCheckboxesByLegend_Duplicate_UsesSetWording()
    {
        var doc = await Render.DocumentAsync(
            g => g.GenerateCheckboxesAsync(Checkboxes("A")),
            g => g.GenerateCheckboxesAsync(Checkboxes("B")));

        var ex = Assert.Throws<ComponentLookupException>(() => doc.GetCheckboxesByLegend("Options"));

        Assert.Equal("More than one set of checkboxes found.", ex.Failure.Summary);
        Assert.Equal("2 sets of checkboxes with the legend \"Options\"", ex.Failure.Found);

        static CheckboxesOptions Checkboxes(string name) => new()
        {
            Name = name,
            Fieldset = new FieldsetOptions { Legend = new FieldsetOptionsLegend { Text = "Options" } },
            Items = [new CheckboxesOptionsItem { Value = "x", Text = "X" }]
        };
    }

    [Fact]
    public async Task Lookups_ScopedToElement_OnlySearchInsideIt()
    {
        var doc = Render.Document(
            "<form id=\"one\">" + await InputHtmlAsync("first-name", "Name") + "</form>" +
            "<form id=\"two\">" + await InputHtmlAsync("second-name", "Name") + "</form>");

        Assert.Throws<ComponentLookupException>(() => doc.GetTextInputByLabel("Name"));

        var second = doc.GetElementById("two")!;
        Assert.Equal("second-name", second.GetTextInputByLabel("Name").Input.Id);

        var ex = Assert.Throws<ComponentLookupException>(() => second.GetTextInputByLabel("Email"));
        Assert.StartsWith("<form id=\"two\">", ex.Failure.Html);
    }

    private static async Task<DateInput> RenderDateAsync(string day, string month, string year)
    {
        var doc = await Render.DocumentAsync(g => g.GenerateDateInputAsync(new DateInputOptions
        {
            Id = "date",
            NamePrefix = "date",
            Fieldset = new FieldsetOptions { Legend = new FieldsetOptionsLegend { Text = "Date" } },
            Items =
            [
                new DateInputOptionsItem { Name = "day", Value = day },
                new DateInputOptionsItem { Name = "month", Value = month },
                new DateInputOptionsItem { Name = "year", Value = year }
            ]
        }));

        return doc.GetDateInputByLegend("Date");
    }

    private static Task<AngleSharp.Html.Dom.IHtmlDocument> RenderFormAsync() =>
        Render.DocumentAsync(
            g => g.GenerateInputAsync(new InputOptions { Id = "name", Name = "Name", Label = new LabelOptions { Text = "Full name" } }),
            g => g.GenerateDateInputAsync(new DateInputOptions
            {
                Id = "dob",
                NamePrefix = "dob",
                Fieldset = new FieldsetOptions { Legend = new FieldsetOptionsLegend { Text = "Date of birth" } }
            }),
            g => g.GenerateRadiosAsync(new RadiosOptions
            {
                Name = "Colour",
                Fieldset = new FieldsetOptions { Legend = new FieldsetOptionsLegend { Text = "Colour" } },
                Items = [new RadiosOptionsItem { Value = "red", Text = "Red" }]
            }));

    private static async Task<string> InputHtmlAsync(string id, string label, string? hint = null) =>
        (await Render.Generator.GenerateInputAsync(new InputOptions
        {
            Id = id,
            Name = id,
            Label = new LabelOptions { Text = label },
            Hint = hint is null ? null : new HintOptions { Text = hint }
        })).GetHtml();
}
