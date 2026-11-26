using AngleSharp.Html.Dom;
using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.Testing.Tests;

public class TaskListTests
{
    [Fact]
    public async Task TaskList_ReadsItems()
    {
        var doc = await RenderTaskListAsync();

        var taskList = doc.TaskList;
        Assert.NotNull(taskList);
        Assert.Equal(["Company Directors", "Financial history", "Payment"], taskList.Items.Select(i => i.Name));

        var directors = taskList.GetItemByName("Company Directors");
        Assert.Equal("Completed", directors.Status);
        Assert.Null(directors.StatusTag);
        Assert.Null(directors.Hint);
        Assert.Equal("/directors", directors.Href);

        var history = doc.GetTaskListItemByName("Financial history");
        Assert.Equal("Include 5 years of the company’s relevant financial information", history.Hint);
        Assert.Equal("Incomplete", history.Status);
        Assert.Equal("Incomplete", history.StatusTag!.Text);
        Assert.Equal("blue", history.StatusTag.Colour);

        var payment = taskList.GetItemByName("Payment");
        Assert.Null(payment.Href);
        Assert.Equal("Cannot start yet", payment.Status);
    }

    [Fact]
    public async Task GetItemByName_NotFound_ThrowsWithNames()
    {
        var doc = await RenderTaskListAsync();

        var ex = Assert.Throws<ComponentLookupException>(() => doc.TaskList!.GetItemByName("Declaration"));

        Assert.Equal("Task list item not found.", ex.Failure.Summary);
        Assert.Equal("a task named \"Declaration\"", ex.Failure.Expected);
        Assert.Equal("tasks with the names \"Company Directors\", \"Financial history\", \"Payment\"", ex.Failure.Found);
        Assert.Null(doc.FindTaskListItemByName("Declaration"));
    }

    private static Task<IHtmlDocument> RenderTaskListAsync() =>
        Render.DocumentAsync(g => g.GenerateTaskListAsync(new TaskListOptions
        {
            Items =
            [
                new TaskListOptionsItem
                {
                    Title = new TaskListOptionsItemTitle { Text = "Company Directors" },
                    Href = "/directors",
                    Status = new TaskListOptionsItemStatus { Text = "Completed" }
                },
                new TaskListOptionsItem
                {
                    Title = new TaskListOptionsItemTitle { Text = "Financial history" },
                    Hint = new TaskListOptionsItemHint { Text = "Include 5 years of the company’s relevant financial information" },
                    Href = "/history",
                    Status = new TaskListOptionsItemStatus
                    {
                        Tag = new TagOptions { Text = "Incomplete", Classes = "govuk-tag--blue" }
                    }
                },
                new TaskListOptionsItem
                {
                    Title = new TaskListOptionsItemTitle { Text = "Payment" },
                    Status = new TaskListOptionsItemStatus { Text = "Cannot start yet", Classes = "govuk-task-list__status--cannot-start-yet" }
                }
            ]
        }));
}
