using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using TodoList.Api.Extensions;

namespace TodoList.Api.Endpoints.Todos;

public static class Endpoint
{
    public static IEndpointRouteBuilder UseTodosEndpoint(this IEndpointRouteBuilder routes)
    {
        var v1 = routes.MapGroup("/v1/todos");
        
        v1.WithTags("/v1/todos");

        v1.MapGet("/", async Task<Ok<ListTodoItemsResponse>> (TodoContext db, bool? isCompleted, int? pageSize, string? nextPageToken, CancellationToken token) =>
        {
            var take  = pageSize switch { null => 10, < 1 or > 100 => 100, _ => pageSize.Value };

            var query = db.TodoItems.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nextPageToken))
            {
                // ReSharper disable once StringCompareToIsCultureSpecific
                query = query.Where(x => x.Id.CompareTo(nextPageToken) > 0);
            }

            if (isCompleted is not null)
            {
                query = query.Where(x => x.IsCompleted == isCompleted);
            }
        
            var results = await query
                .OrderBy(w => w.Id)
                .Take(take)
                .AsNoTracking()
                .ToListAsync(token);
            
            var response = new ListTodoItemsResponse
            {
                NextPageToken = results.LastOrDefault()?.Id,
                Items = results.Select(x => x.ToDescription()).ToList()
            };

            return TypedResults.Ok(response);
        })
        .WithName("ListTodos")
        .WithSummary("Lists todo items using pagination")
        .WithDescription("Lists todo items using pagination");

        v1.MapGet("/{id}", async Task<Results<Ok<DescribeTodoItemResponse>, NotFound>> (TodoContext db, string id, CancellationToken token) =>
        {
            var entity = await db.TodoItems.FindAsync(id, token);
            return entity switch
            {
                null => TypedResults.NotFound(),
                _ => TypedResults.Ok(entity.ToDescription())
            };
        })
        .WithName("DescribeTodo")
        .WithSummary("Describes a todo item")
        .WithDescription("Describes a todo item");

        v1.MapPost("/", async Task<Results<ValidationProblem, Created<CreateTodoItemResponse>>> (LinkGenerator links, TodoContext db, CreateTodoItemRequest request, CancellationToken token) =>
        {
            try
            {
                var entity = new TodoItemEntity
                {
                    Id = IdGenerator.NewId(),
                    Description = request.Description!,
                    IsCompleted = request.IsCompleted!.Value,
                };

                db.TodoItems.Add(entity);
                await db.SaveChangesAsync(token);

                var url = links.DescribeTodoUrl(entity.Id);
                var response = new CreateTodoItemResponse
                {
                    Item = entity.ToDescription()
                };
                return TypedResults.Created(url, response);
            }
            catch (DbUpdateException ex) when(ex.InnerException is MySqlException { ErrorCode: MySqlErrorCode.DuplicateKeyEntry })
            {
                var errors = new Dictionary<string, string[]> { { "Description", new[] { "The Description field must be unique" } } };
                return TypedResults.ValidationProblem(errors);
            }
        })
        .WithParameterValidation()
        .WithName("CreateTodo")
        .WithSummary("Creates a todo item")
        .WithDescription("Creates a todo item");

        v1.MapPut("/{id}", async Task<Results<ValidationProblem, NoContent, NotFound>> (TodoContext db, string id, UpdateTodoItemRequest request, CancellationToken token) =>
        {
            try
            {
                var rows = await db.TodoItems.Where(x => x.Id == id)
                    .ExecuteUpdateAsync(u => u
                            .SetProperty(c => c.Description, request.Description!)
                            .SetProperty(c => c.IsCompleted, request.IsCompleted!.Value),
                        cancellationToken: token
                    );

                return rows == 0 ? TypedResults.NotFound() : TypedResults.NoContent();
            }
            catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
            {
                var errors = new Dictionary<string, string[]> { { "Description", new[] { "The Description field must be unique" } } };
                return TypedResults.ValidationProblem(errors);
            }
        })
        .WithParameterValidation()
        .WithName("UpdateTodo")
        .WithSummary("Updates a todo item")
        .WithDescription("Updates a todo item");

        v1.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (string id, TodoContext db, CancellationToken token) =>
        {
            var rows = await db.TodoItems.Where(x => x.Id == id).ExecuteDeleteAsync(token);
            return rows == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        })
        .WithName("DeleteTodo")
        .WithSummary("Deletes a todo item")
        .WithDescription("Deletes a todo item");

        return v1;
    }
}