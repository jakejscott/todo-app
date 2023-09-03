namespace TodoList.Api.UnitTests.Endpoints.Todos;

public class GetTodoTests
{
    [Fact]
    public async Task ShouldReturnItem()
    {
        await using var app = new TestApplication();
        await using var db = app.CreateTodoContext();

        var id = IdGenerator.NewId();

        var entity = new TodoItemEntity
        {
            Id = id,
            Description = "Description complete",
            IsCompleted = true
        };

        db.TodoItems.Add(entity);
        await db.SaveChangesAsync();

        var client = app.CreateClient();
        var response = await client.GetFromJsonAsync<DescribeTodoItemResponse>($"/v1/todos/{id}");
        
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(entity);
    }

    [Fact]
    public async Task ShouldReturnNotFound()
    {
        await using var app = new TestApplication();
        await using var db = app.CreateTodoContext();

        var client = app.CreateClient();
        
        var id = IdGenerator.NewId();
        
        var httpResponse = await client.GetAsync($"/v1/todos/{id}");
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}