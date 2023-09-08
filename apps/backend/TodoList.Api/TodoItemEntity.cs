using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TodoList.Api;

[Table("todo")]
[Index(nameof(Description), IsUnique = true)]
public class TodoItemEntity
{
    [Column("id")]
    public required string Id { get; set; }
    
    [Column("description")]
    [MaxLength(length: 50)]
    public required string Description { get; set; }
    
    [Column("is_completed")]
    public required bool IsCompleted { get; set; }
}