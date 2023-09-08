namespace TodoList.Api.UnitTests.Endpoints.Todos;

public class UpdateTodoTests
{
    [Fact]
    public async Task ShouldReturnNotFound()
    {
        await using var app = new TestApplication();
        await using var db = await app.CreateTodoContext();

        var id = IdGenerator.NewId();

        var client = app.CreateClient();
        
        var request = new UpdateTodoItemRequest
        {
            Description = "Description updated",
            IsCompleted = true
        };
        
        var httpResponse = await client.PutAsJsonAsync($"/v1/todos/{id}", request);
        httpResponse.Should().NotBeNull();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ShouldReturnNotContentIfValid()
    {
        await using var app = new TestApplication();
        await using var db = await app.CreateTodoContext();

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
        
        var request = new UpdateTodoItemRequest
        {
            Description = "Description updated",
            IsCompleted = true
        };
        
        var httpResponse = await client.PutAsJsonAsync($"/v1/todos/{id}", request);
        httpResponse.Should().NotBeNull();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
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
        
        var entity2 = new TodoItemEntity
        {
            Id = IdGenerator.NewId(),
            Description = "Description 2",
            IsCompleted = false
        };

        db.TodoItems.Add(entity1);
        db.TodoItems.Add(entity2);
        await db.SaveChangesAsync();

        var client = app.CreateClient();
        
        var request = new UpdateTodoItemRequest
        {
            Description = entity1.Description,
            IsCompleted = true
        };
        
        var httpResponse = await client.PutAsJsonAsync($"/v1/todos/{entity2.Id}", request);
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
        
        var id = IdGenerator.NewId();

        var entity = new TodoItemEntity
        {
            Id = id,
            Description = "Description",
            IsCompleted = false
        };

        db.TodoItems.Add(entity);
        await db.SaveChangesAsync();
    
        var request = new UpdateTodoItemRequest
        {
            Description = description,
            IsCompleted = true
        };
    
        var client = app.CreateClient();
        
        var httpResponse = await client.PutAsJsonAsync($"/v1/todos/{id}", request);
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
        
        var id = IdGenerator.NewId();

        var entity = new TodoItemEntity
        {
            Id = id,
            Description = "Description",
            IsCompleted = false
        };

        db.TodoItems.Add(entity);
        await db.SaveChangesAsync();
    
        var request = new CreateTodoItemRequest
        {
            Description = "Description",
            IsCompleted = isCompleted
        };
    
        var client = app.CreateClient();
        
        var httpResponse = await client.PutAsJsonAsync($"/v1/todos/{id}", request);
        httpResponse.Should().NotBeNull();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var problem = await httpResponse.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        var field = problem!.Errors.Single(x => x.Key == "IsCompleted");
        var error = field.Value.Single();
        error.Should().Be(message);
    }
}