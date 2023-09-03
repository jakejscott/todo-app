namespace TodoList.Api.UnitTests.Endpoints.Todos;

public class ListTodosTests
{
    [Fact]
    public async Task ShouldReturnEmptyItems()
    {
        await using var app = new TestApplication();
        await using var db = app.CreateTodoContext();
        
        var client = app.CreateClient();
        var response = await client.GetFromJsonAsync<ListTodoItemsResponse>("/v1/todos");
        response.Should().NotBeNull();
        response!.Items.Should().BeEmpty();
    }
    
    [Fact]
    public async Task ShouldReturnCompletedItems()
    {
        await using var app = new TestApplication();
        await using var db = app.CreateTodoContext();
        
        db.TodoItems.Add(new TodoItemEntity
        {
            Id = IdGenerator.NewId(),
            Description = "Description complete",
            IsCompleted = true
        });

        db.TodoItems.Add(new TodoItemEntity
        {
            Id = IdGenerator.NewId(),
            Description = "Description not complete",
            IsCompleted = false
        });
        
        await db.SaveChangesAsync();
        
        var client = app.CreateClient();
        var response = await client.GetFromJsonAsync<ListTodoItemsResponse>("/v1/todos?isCompleted=true");
        
        response.Should().NotBeNull();
        response!.Items.Count.Should().Be(1);

        response.Items.Single().IsCompleted.Should().Be(true);
        response.Items.Single().Description.Should().Be("Description complete");
    }
    
    [Fact]
    public async Task ShouldReturnNotCompletedItems()
    {
        await using var app = new TestApplication();
        await using var db = app.CreateTodoContext();
        
        db.TodoItems.Add(new TodoItemEntity
        {
            Id = IdGenerator.NewId(),
            Description = "Description complete",
            IsCompleted = true
        });

        db.TodoItems.Add(new TodoItemEntity
        {
            Id = IdGenerator.NewId(),
            Description = "Description not complete",
            IsCompleted = false
        });
        
        await db.SaveChangesAsync();
        
        var client = app.CreateClient();
        var response = await client.GetFromJsonAsync<ListTodoItemsResponse>("/v1/todos?isCompleted=false");
        
        response.Should().NotBeNull();
        response!.Items.Count.Should().Be(1);
        
        response.Items.Single().IsCompleted.Should().Be(false);
        response.Items.Single().Description.Should().Be("Description not complete");
    }
    
    [Fact]
    public async Task ShouldReturnItemsPaginated()
    {
        await using var app = new TestApplication();
        await using var db = app.CreateTodoContext();

        const int total = 100;
        for (var i = 0; i < total; i++)
        {
            // NOTE: Ulid are not guaranteed to have lexicographic within a ms, so add 1ms delay. 
            await Task.Delay(1);
            var id = IdGenerator.NewId();
            
            db.TodoItems.Add(new TodoItemEntity
            {
                Id = id,
                Description = $"Description {i}",
                IsCompleted = i % 2 == 0,
            });
            
            await db.SaveChangesAsync();
        }
        
        var client = app.CreateClient();

        string? nextPageToken = null;
        const int pageSize = 10;
        var count = 0;
        do
        {
            var response = await client.GetFromJsonAsync<ListTodoItemsResponse>($"/v1/todos?pageSize={pageSize}&nextPageToken={nextPageToken}");
            response.Should().NotBeNull();

            if (response!.Items.Count == 0)
            {
                response.NextPageToken.Should().BeNull();
                break;
            }

            response.Items.Should().NotBeEmpty();
            response.Items.Count.Should().Be(pageSize);
            
            nextPageToken = response.NextPageToken;
            count += pageSize;
        } while (nextPageToken is not null);

        count.Should().Be(total);
    }
}