using System.Diagnostics.CodeAnalysis;

namespace CoProject.Server.Model;

[ExcludeFromCodeCoverage]
public static class Extensions
{
    public static IActionResult ToActionResult(this Status status)
    {
        return status switch
        {
            Status.NotFound => new NotFoundResult(),
            Status.Updated => new NoContentResult(),
            Status.Deleted => new NoContentResult(),
            _ => throw new NotSupportedException($"{status} not yet supported")
        };
    }

    public static ActionResult<T> ToActionResult<T>(this Option<T> option) where T : class
    {
        return option.IsSome ? option.Value : new NotFoundResult();
    }
}