using Microsoft.EntityFrameworkCore;

namespace TodoList.Api;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options) : base(options)
    {
    }

    public DbSet<TodoItemEntity> TodoItems { get; set; }
}