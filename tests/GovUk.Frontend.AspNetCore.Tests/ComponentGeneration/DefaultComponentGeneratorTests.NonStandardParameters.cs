using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.Tests.ComponentGeneration;

public partial class DefaultComponentGeneratorTests
{
    [Fact]
    public async Task Breadcrumbs_ItemAttributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new BreadcrumbsOptions
        {
            Items =
            [
                new BreadcrumbsOptionsItem
                {
                    Text = "Home",
                    Href = "/",
                    ItemAttributes = new AttributeCollection { { "data-test", "item-attr" } }
                }
            ]
        };

        // Act
        var result = await _componentGenerator.GenerateBreadcrumbsAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"item-attr\"", html);
    }

    [Fact]
    public async Task CharacterCount_CountMessage_Attributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new CharacterCountOptions
        {
            Id = "test-id",
            Name = "test",
            MaxLength = 10,
            CountMessage = new CharacterCountCountOptionsMessage
            {
                Attributes = new AttributeCollection { { "data-test", "count-message-attr" } }
            }
        };

        // Act
        var result = await _componentGenerator.GenerateCharacterCountAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"count-message-attr\"", html);
    }

    [Fact]
    public async Task CookieBanner_Message_HeadingAttributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new CookieBannerOptions
        {
            Messages =
            [
                new CookieBannerOptionsMessage
                {
                    HeadingText = "Cookies",
                    Text = "We use cookies",
                    HeadingAttributes = new AttributeCollection { { "data-test", "heading-attr" } }
                }
            ]
        };

        // Act
        var result = await _componentGenerator.GenerateCookieBannerAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"heading-attr\"", html);
    }

    [Fact]
    public async Task CookieBanner_Message_ContentAttributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new CookieBannerOptions
        {
            Messages =
            [
                new CookieBannerOptionsMessage
                {
                    Text = "We use cookies",
                    ContentAttributes = new AttributeCollection { { "data-test", "content-attr" } }
                }
            ]
        };

        // Act
        var result = await _componentGenerator.GenerateCookieBannerAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"content-attr\"", html);
    }

    [Fact]
    public async Task CookieBanner_Message_ActionsAttributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new CookieBannerOptions
        {
            Messages =
            [
                new CookieBannerOptionsMessage
                {
                    Text = "We use cookies",
                    Actions =
                    [
                        new CookieBannerOptionsMessageAction { Text = "Accept", Href = "#" }
                    ],
                    ActionsAttributes = new AttributeCollection { { "data-test", "actions-attr" } }
                }
            ]
        };

        // Act
        var result = await _componentGenerator.GenerateCookieBannerAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"actions-attr\"", html);
    }

    [Fact]
    public async Task Details_SummaryAttributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new DetailsOptions
        {
            SummaryText = "Summary",
            Text = "Details text",
            SummaryAttributes = new AttributeCollection { { "data-test", "summary-attr" } }
        };

        // Act
        var result = await _componentGenerator.GenerateDetailsAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"summary-attr\"", html);
    }

    [Fact]
    public async Task Details_TextAttributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new DetailsOptions
        {
            SummaryText = "Summary",
            Text = "Details text",
            TextAttributes = new AttributeCollection { { "data-test", "text-attr" } }
        };

        // Act
        var result = await _componentGenerator.GenerateDetailsAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"text-attr\"", html);
    }

    [Fact]
    public async Task ErrorSummary_TitleAttributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new ErrorSummaryOptions
        {
            TitleText = "There is a problem",
            TitleAttributes = new AttributeCollection { { "data-test", "title-attr" } }
        };

        // Act
        var result = await _componentGenerator.GenerateErrorSummaryAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"title-attr\"", html);
    }

    [Fact]
    public async Task ErrorSummary_DescriptionAttributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new ErrorSummaryOptions
        {
            TitleText = "There is a problem",
            DescriptionText = "Please fix the errors",
            DescriptionAttributes = new AttributeCollection { { "data-test", "description-attr" } }
        };

        // Act
        var result = await _componentGenerator.GenerateErrorSummaryAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"description-attr\"", html);
    }

    [Fact]
    public async Task ErrorSummary_ErrorItem_ItemAttributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new ErrorSummaryOptions
        {
            TitleText = "There is a problem",
            ErrorList =
            [
                new ErrorSummaryOptionsErrorItem
                {
                    Text = "Error message",
                    Href = "#error",
                    ItemAttributes = new AttributeCollection { { "data-test", "item-attr" } }
                }
            ]
        };

        // Act
        var result = await _componentGenerator.GenerateErrorSummaryAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"item-attr\"", html);
    }

    [Fact]
    public async Task Fieldset_Legend_Attributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new FieldsetOptions
        {
            Legend = new FieldsetOptionsLegend
            {
                Text = "Legend text",
                Attributes = new AttributeCollection { { "data-test", "legend-attr" } }
            }
        };

        // Act
        var result = await _componentGenerator.GenerateFieldsetAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"legend-attr\"", html);
    }

    [Fact]
    public async Task Footer_Meta_Attributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new FooterOptions
        {
            Meta = new FooterOptionsMeta
            {
                Text = "Meta text",
                Attributes = new AttributeCollection { { "data-test", "meta-attr" } }
            }
        };

        // Act
        var result = await _componentGenerator.GenerateFooterAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"meta-attr\"", html);
    }

    [Fact]
    public async Task Footer_Meta_ContentAttributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new FooterOptions
        {
            Meta = new FooterOptionsMeta
            {
                Text = "Meta text",
                ContentAttributes = new AttributeCollection { { "data-test", "content-attr" } }
            }
        };

        // Act
        var result = await _componentGenerator.GenerateFooterAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"content-attr\"", html);
    }

    [Fact]
    public async Task Footer_Meta_ItemsAttributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new FooterOptions
        {
            Meta = new FooterOptionsMeta
            {
                Items =
                [
                    new FooterOptionsMetaItem { Text = "Item", Href = "#" }
                ],
                ItemsAttributes = new AttributeCollection { { "data-test", "items-attr" } }
            }
        };

        // Act
        var result = await _componentGenerator.GenerateFooterAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"items-attr\"", html);
    }

    [Fact]
    public async Task Footer_MetaItem_Html_IsIncludedInOutput()
    {
        // Arrange
        var options = new FooterOptions
        {
            Meta = new FooterOptionsMeta
            {
                Items =
                [
                    new FooterOptionsMetaItem
                    {
                        Html = "<span data-test=\"item-html\">Custom HTML</span>",
                        Href = "#"
                    }
                ]
            }
        };

        // Act
        var result = await _componentGenerator.GenerateFooterAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"item-html\"", html);
    }

    [Fact]
    public async Task Footer_MetaItem_ItemAttributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new FooterOptions
        {
            Meta = new FooterOptionsMeta
            {
                Items =
                [
                    new FooterOptionsMetaItem
                    {
                        Text = "Item",
                        Href = "#",
                        ItemAttributes = new AttributeCollection { { "data-test", "item-attr" } }
                    }
                ]
            }
        };

        // Act
        var result = await _componentGenerator.GenerateFooterAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"item-attr\"", html);
    }

    [Fact]
    public async Task Footer_Navigation_Attributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new FooterOptions
        {
            Navigation =
            [
                new FooterOptionsNavigation
                {
                    Title = "Section",
                    Items =
                    [
                        new FooterOptionsNavigationItem { Text = "Item", Href = "#" }
                    ],
                    Attributes = new AttributeCollection { { "data-test", "nav-attr" } }
                }
            ]
        };

        // Act
        var result = await _componentGenerator.GenerateFooterAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"nav-attr\"", html);
    }

    [Fact]
    public async Task Footer_Navigation_ItemsAttributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new FooterOptions
        {
            Navigation =
            [
                new FooterOptionsNavigation
                {
                    Title = "Section",
                    Items =
                    [
                        new FooterOptionsNavigationItem { Text = "Item", Href = "#" }
                    ],
                    ItemsAttributes = new AttributeCollection { { "data-test", "items-attr" } }
                }
            ]
        };

        // Act
        var result = await _componentGenerator.GenerateFooterAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"items-attr\"", html);
    }

    [Fact]
    public async Task Footer_Navigation_TitleAttributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new FooterOptions
        {
            Navigation =
            [
                new FooterOptionsNavigation
                {
                    Title = "Section",
                    Items =
                    [
                        new FooterOptionsNavigationItem { Text = "Item", Href = "#" }
                    ],
                    TitleAttributes = new AttributeCollection { { "data-test", "title-attr" } }
                }
            ]
        };

        // Act
        var result = await _componentGenerator.GenerateFooterAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"title-attr\"", html);
    }

    [Fact]
    public async Task Footer_NavigationItem_Html_IsIncludedInOutput()
    {
        // Arrange
        var options = new FooterOptions
        {
            Navigation =
            [
                new FooterOptionsNavigation
                {
                    Title = "Section",
                    Items =
                    [
                        new FooterOptionsNavigationItem
                        {
                            Html = "<span data-test=\"item-html\">Custom HTML</span>",
                            Href = "#"
                        }
                    ]
                }
            ]
        };

        // Act
        var result = await _componentGenerator.GenerateFooterAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"item-html\"", html);
    }

    [Fact]
    public async Task Footer_NavigationItem_ItemAttributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new FooterOptions
        {
            Navigation =
            [
                new FooterOptionsNavigation
                {
                    Title = "Section",
                    Items =
                    [
                        new FooterOptionsNavigationItem
                        {
                            Text = "Item",
                            Href = "#",
                            ItemAttributes = new AttributeCollection { { "data-test", "item-attr" } }
                        }
                    ]
                }
            ]
        };

        // Act
        var result = await _componentGenerator.GenerateFooterAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"item-attr\"", html);
    }

    [Fact]
    public async Task Footer_ContentLicence_Attributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new FooterOptions
        {
            ContentLicence = new FooterOptionsContentLicence
            {
                Text = "License text",
                Attributes = new AttributeCollection { { "data-test", "license-attr" } }
            }
        };

        // Act
        var result = await _componentGenerator.GenerateFooterAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"license-attr\"", html);
    }

    [Fact]
    public async Task Footer_Copyright_Attributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new FooterOptions
        {
            Copyright = new FooterOptionsCopyright
            {
                Text = "Copyright text",
                Attributes = new AttributeCollection { { "data-test", "copyright-attr" } }
            }
        };

        // Act
        var result = await _componentGenerator.GenerateFooterAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"copyright-attr\"", html);
    }

    [Fact]
    public async Task Header_ContainerAttributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new HeaderOptions
        {
            ServiceName = "Service",
            ContainerAttributes = new AttributeCollection { { "data-test", "container-attr" } }
        };

        // Act
        var result = await _componentGenerator.GenerateHeaderAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"container-attr\"", html);
    }

    [Fact]
    public async Task Header_NavigationAttributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new HeaderOptions
        {
            Navigation =
            [
                new HeaderOptionsNavigationItem { Text = "Item", Href = "#" }
            ],
            NavigationAttributes = new AttributeCollection { { "data-test", "nav-attr" } }
        };

        // Act
        var result = await _componentGenerator.GenerateHeaderAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"nav-attr\"", html);
    }

    [Fact]
    public async Task Pagination_Previous_ContainerAttributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new PaginationOptions
        {
            Previous = new PaginationOptionsPrevious
            {
                Href = "/previous",
                ContainerAttributes = new AttributeCollection { { "data-test", "prev-container-attr" } }
            }
        };

        // Act
        var result = await _componentGenerator.GeneratePaginationAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"prev-container-attr\"", html);
    }

    [Fact]
    public async Task Pagination_Next_ContainerAttributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new PaginationOptions
        {
            Next = new PaginationOptionsNext
            {
                Href = "/next",
                ContainerAttributes = new AttributeCollection { { "data-test", "next-container-attr" } }
            }
        };

        // Act
        var result = await _componentGenerator.GeneratePaginationAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"next-container-attr\"", html);
    }

    [Fact]
    public async Task Panel_TitleAttributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new PanelOptions
        {
            TitleText = "Title",
            TitleAttributes = new AttributeCollection { { "data-test", "title-attr" } }
        };

        // Act
        var result = await _componentGenerator.GeneratePanelAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"title-attr\"", html);
    }

    [Fact]
    public async Task Panel_BodyAttributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new PanelOptions
        {
            TitleText = "Title",
            Text = "Body",
            BodyAttributes = new AttributeCollection { { "data-test", "body-attr" } }
        };

        // Act
        var result = await _componentGenerator.GeneratePanelAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"body-attr\"", html);
    }

    [Fact]
    public async Task ServiceNavigation_NavigationAttributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new ServiceNavigationOptions
        {
            Navigation =
            [
                new ServiceNavigationOptionsNavigationItem { Text = "Item", Href = "#" }
            ],
            NavigationAttributes = new AttributeCollection { { "data-test", "nav-attr" } }
        };

        // Act
        var result = await _componentGenerator.GenerateServiceNavigationAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"nav-attr\"", html);
    }

    [Fact]
    public async Task SummaryList_CardTitle_Attributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new SummaryListOptions
        {
            Card = new SummaryListOptionsCard
            {
                Title = new SummaryListOptionsCardTitle
                {
                    Text = "Title",
                    Attributes = new AttributeCollection { { "data-test", "title-attr" } }
                }
            },
            Rows =
            [
                new SummaryListOptionsRow
                {
                    Key = new SummaryListOptionsRowKey { Text = "Key" },
                    Value = new SummaryListOptionsRowValue { Text = "Value" }
                }
            ]
        };

        // Act
        var result = await _componentGenerator.GenerateSummaryListAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"title-attr\"", html);
    }

    [Fact]
    public async Task SummaryList_Row_Attributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new SummaryListOptions
        {
            Rows =
            [
                new SummaryListOptionsRow
                {
                    Key = new SummaryListOptionsRowKey { Text = "Key" },
                    Value = new SummaryListOptionsRowValue { Text = "Value" },
                    Attributes = new AttributeCollection { { "data-test", "row-attr" } }
                }
            ]
        };

        // Act
        var result = await _componentGenerator.GenerateSummaryListAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"row-attr\"", html);
    }

    [Fact]
    public async Task SummaryList_RowKey_Attributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new SummaryListOptions
        {
            Rows =
            [
                new SummaryListOptionsRow
                {
                    Key = new SummaryListOptionsRowKey
                    {
                        Text = "Key",
                        Attributes = new AttributeCollection { { "data-test", "key-attr" } }
                    },
                    Value = new SummaryListOptionsRowValue { Text = "Value" }
                }
            ]
        };

        // Act
        var result = await _componentGenerator.GenerateSummaryListAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"key-attr\"", html);
    }

    [Fact]
    public async Task SummaryList_RowValue_Attributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new SummaryListOptions
        {
            Rows =
            [
                new SummaryListOptionsRow
                {
                    Key = new SummaryListOptionsRowKey { Text = "Key" },
                    Value = new SummaryListOptionsRowValue
                    {
                        Text = "Value",
                        Attributes = new AttributeCollection { { "data-test", "value-attr" } }
                    }
                }
            ]
        };

        // Act
        var result = await _componentGenerator.GenerateSummaryListAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"value-attr\"", html);
    }

    [Fact]
    public async Task SummaryList_RowActions_Attributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new SummaryListOptions
        {
            Rows =
            [
                new SummaryListOptionsRow
                {
                    Key = new SummaryListOptionsRowKey { Text = "Key" },
                    Value = new SummaryListOptionsRowValue { Text = "Value" },
                    Actions = new SummaryListOptionsRowActions
                    {
                        Items =
                        [
                            new SummaryListOptionsRowActionsItem { Text = "Change", Href = "#" }
                        ],
                        Attributes = new AttributeCollection { { "data-test", "actions-attr" } }
                    }
                }
            ]
        };

        // Act
        var result = await _componentGenerator.GenerateSummaryListAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"actions-attr\"", html);
    }

    [Fact]
    public async Task SummaryList_CardActions_Attributes_IsIncludedInOutput()
    {
        // Arrange
        var options = new SummaryListOptions
        {
            Card = new SummaryListOptionsCard
            {
                Title = new SummaryListOptionsCardTitle { Text = "Title" },
                Actions = new SummaryListOptionsCardActions
                {
                    Items =
                    [
                        new SummaryListOptionsCardActionsItem { Text = "Delete", Href = "#" }
                    ],
                    Attributes = new AttributeCollection { { "data-test", "card-actions-attr" } }
                }
            },
            Rows =
            [
                new SummaryListOptionsRow
                {
                    Key = new SummaryListOptionsRowKey { Text = "Key" },
                    Value = new SummaryListOptionsRowValue { Text = "Value" }
                }
            ]
        };

        // Act
        var result = await _componentGenerator.GenerateSummaryListAsync(options);
        var html = result.GetHtml();

        // Assert
        Assert.Contains("data-test=\"card-actions-attr\"", html);
    }
}
