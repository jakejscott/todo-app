namespace TodoList.Api.UnitTests.Endpoints.Todos;

public class DeleteTodoTests
{
    [Fact]
    public async Task ShouldReturnOk()
    {
        await using var app = new TestApplication();
        await using var db = app.CreateTodoContext();
        
        var id = IdGenerator.NewId();

        var entity = new TodoItemEntity
        {
            Id = id,
            Description = "Description",
            IsCompleted = false
        };

        db.TodoItems.Add(entity);
        await db.SaveChangesAsync();

        var client = app.CreateClient();
        
        var httpResponse = await client.DeleteAsync($"/v1/todos/{id}");
        httpResponse.Should().NotBeNull();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task ShouldReturnNotFound()
    {
        await using var app = new TestApplication();
        await using var db = app.CreateTodoContext();
        
        var id = IdGenerator.NewId();

        var client = app.CreateClient();
        
        var httpResponse = await client.DeleteAsync($"/v1/todos/{id}");
        httpResponse.Should().NotBeNull();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}