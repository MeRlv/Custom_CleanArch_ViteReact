using Microsoft.AspNetCore.Http.HttpResults;
using YourProjectName.Application.Common.Exceptions;
using YourProjectName.Application.Common.Abstract;
using System.Net;

namespace YourProjectName.Web.Infrastructure;

public abstract class EndpointGroupBase
{
    public virtual string? GroupName { get; }
    public abstract void Map(RouteGroupBuilder groupBuilder);

    protected IResult HandleException(Exception ex)
    {
        switch (ex)
        {
            case HTTPException httpEx:
                return HandleHttpException(httpEx);

            case ValidationException validationEx:
                return TypedResults.BadRequest(new 
                { 
                    Message = validationEx.Message,
                    Errors = validationEx.Errors 
                });

            case System.ArgumentException argEx:
                return TypedResults.BadRequest(new { Message = argEx.Message });

            case System.InvalidOperationException invalidOpEx:
                return TypedResults.BadRequest(new { Message = invalidOpEx.Message });

            default:
                // Log l'exception pour le debug
                Console.WriteLine($"Unhandled exception: {ex}");
                return TypedResults.BadRequest(new { Message = "Une erreur inattendue s'est produite." });
        }
    }

    private IResult HandleHttpException(HTTPException httpEx)
    {
        var response = new { Message = httpEx.Message };
        
        return httpEx.StatusCode switch
        {
            HttpStatusCode.BadRequest => TypedResults.BadRequest(response),
            HttpStatusCode.NotFound => TypedResults.NotFound(response),
            HttpStatusCode.Forbidden => TypedResults.Forbid(),
            HttpStatusCode.Unauthorized => TypedResults.Unauthorized(),
            _ => TypedResults.BadRequest(response)
        };
    }
}
