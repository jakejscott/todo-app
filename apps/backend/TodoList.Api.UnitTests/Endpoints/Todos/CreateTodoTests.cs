namespace TodoList.Api.UnitTests.Endpoints.Todos;

public class CreateTodoTests
{
    [Fact]
    public async Task ShouldReturnItemIfValid()
    {
        await using var app = new TestApplication();
        await using var db = await app.CreateTodoContext();

        var request = new CreateTodoItemRequest
        {
            Description = "Description",
            IsCompleted = true
        };

        var client = app.CreateClient();
        
        var httpResponse = await client.PostAsJsonAsync("/v1/todos/", request);
        httpResponse.Should().NotBeNull();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var response = await httpResponse.Content.ReadFromJsonAsync<CreateTodoItemResponse>();
        response.Should().NotBeNull();
        response!.Item.Should().NotBeNull();
        response.Item.Should().BeEquivalentTo(request);
    }
    
    [Fact]
    public async Task ShouldReturnBadRequestIfDuplicateDescription()
    {
        await using var app = new TestApplication();
        await using var db = await app.CreateTodoContext();

        var entity1 = new TodoItemEntity
        {
            Id = IdGenerator.NewId(),
            Description = "Description 1",
            IsCompleted = false
        };
        
        db.TodoItems.Add(entity1);
        await db.SaveChangesAsync();

        var client = app.CreateClient();
        
        var request = new CreateTodoItemRequest
        {
            Description = entity1.Description,
            IsCompleted = true
        };
        
        var httpResponse = await client.PostAsJsonAsync($"/v1/todos/", request);
        httpResponse.Should().NotBeNull();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var problem = await httpResponse.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        var field = problem!.Errors.Single(x => x.Key == "Description");
        var error = field.Value.Single();
        error.Should().Be("The Description field must be unique");
    }
    
    [Theory]
    [InlineData(null, "The Description field is required.")]
    [InlineData("", "The Description field is required.")]
    [InlineData("a", "The field Description must be a string with a minimum length of 3 and a maximum length of 50.")]
    public async Task ShouldReturnBadRequestIfDescriptionNotValid(string description, string message)
    {
        await using var app = new TestApplication();
        await using var db = await app.CreateTodoContext();

        var request = new CreateTodoItemRequest
        {
            Description = description,
            IsCompleted = true
        };

        var client = app.CreateClient();
        
        var httpResponse = await client.PostAsJsonAsync("/v1/todos/", request);
        httpResponse.Should().NotBeNull();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var problem = await httpResponse.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        var field = problem!.Errors.Single(x => x.Key == "Description");
        var error = field.Value.Single();
        error.Should().Be(message);
    }
    
    [Theory]
    [InlineData(null, "The IsCompleted field is required.")]
    public async Task ShouldReturnBadRequestIfIsCompletedIsNotValid(bool? isCompleted, string message)
    {
        await using var app = new TestApplication();
        await using var db = await app.CreateTodoContext();

        var request = new CreateTodoItemRequest
        {
            Description = "Description",
            IsCompleted = isCompleted
        };

        var client = app.CreateClient();
        
        var httpResponse = await client.PostAsJsonAsync("/v1/todos/", request);
        httpResponse.Should().NotBeNull();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var problem = await httpResponse.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        var field = problem!.Errors.Single(x => x.Key == "IsCompleted");
        var error = field.Value.Single();
        error.Should().Be(message);
    }
}