namespace TodoList.Api.Endpoints.Todos;

public static class Mapper
{
    public static DescribeTodoItemResponse ToDescription(this TodoItemEntity entity)
    {
        return new()
        {
            Id = entity.Id,
            Description = entity.Description,
            IsCompleted = entity.IsCompleted,
        };
    }
}