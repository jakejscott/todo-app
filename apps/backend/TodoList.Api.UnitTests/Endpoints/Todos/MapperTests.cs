namespace TodoList.Api.UnitTests.Endpoints.Todos;

public class MapperTests
{
    [Fact]
    public void EntityAsItemTest()
    {
        TodoItemEntity entity = new ()
        {
            Id = IdGenerator.NewId(),
            Description = "Description",
            IsCompleted = true
        };

        DescribeTodoItemResponse response = entity.AsItem();

        response.Should().BeEquivalentTo(entity);
    }
}