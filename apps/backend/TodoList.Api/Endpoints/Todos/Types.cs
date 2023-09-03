using System.ComponentModel.DataAnnotations;

namespace TodoList.Api.Endpoints.Todos;

/// <summary>
/// Create todo request payload
/// </summary>
public record CreateTodoItemRequest
{
    /// <summary>
    /// The description of the todo item
    /// </summary>
    [StringLength(maximumLength: 50, MinimumLength = 3)]
    [Required]
    public string? Description { get; set; }

    /// <summary>
    /// Is the todo item completed?
    /// </summary>
    [Required]
    public bool? IsCompleted { get; set; }
}

/// <summary>
/// Create todo response payload
/// </summary>
public class CreateTodoItemResponse
{
    [Required]
    public required DescribeTodoItemResponse Item { get; set; }
}

/// <summary>
/// Update todo request payload
/// </summary>
public class UpdateTodoItemRequest
{
    /// <summary>
    /// The description of the todo item
    /// </summary>
    [Required]
    [StringLength(maximumLength: 50, MinimumLength = 3)]
    public string? Description { get; set; }

    /// <summary>
    /// Is the todo item completed?
    /// </summary>
    [Required]
    public bool? IsCompleted { get; set; }
}

/// <summary>
/// Describe todo items response payload
/// </summary>
public class DescribeTodoItemResponse
{
    /// <summary>
    /// The Id of the todo item
    /// </summary>
    [Required]
    public required string Id { get; set; }

    /// <summary>
    /// The description of the todo item
    /// </summary>
    [Required]
    public required string Description { get; set; }

    /// <summary>
    /// Is the todo item completed?
    /// </summary>
    [Required]
    public required bool IsCompleted { get; set; }
}

/// <summary>
/// List todo items response
/// </summary>
public class ListTodoItemsResponse
{
    /// <summary>
    /// The token to use to get the next page of results
    /// </summary>
    public string? NextPageToken { get; set; }
    
    /// <summary>
    /// The list of todo items
    /// </summary>
    [Required]
    public required List<DescribeTodoItemResponse> Items { get; set; } = new();
}