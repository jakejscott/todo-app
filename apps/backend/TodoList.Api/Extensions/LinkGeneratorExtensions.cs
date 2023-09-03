namespace TodoList.Api.Extensions;

public static class LinkGeneratorExtensions
{
    public static string DescribeTodoUrl(this LinkGenerator links, string id)
    {
        var url = links.GetPathByName("DescribeTodo", values: new() { { "id", id } });

        if (url is null)
        {
            throw new Exception("DescribeTodo url was null");
        }

        return url;
    }
}