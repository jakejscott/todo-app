namespace TodoList.Api.Extensions;

public static class IdGenerator
{
    public static string NewId()
    {
        // NOTE: Using Ulids as they are lexicographically sortable. 
        return Ulid.NewUlid().ToString()!.ToLower();
    }
}