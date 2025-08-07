using System.Net;
using YourProjectName.Application.Common.Abstract;

namespace YourProjectName.Application.Common.Exceptions;
public class BadRequestException : HTTPException
{
    public BadRequestException()
        : base(HttpStatusCode.BadRequest, "Bad request.")
    { }

    public BadRequestException(string message)
        : base(HttpStatusCode.BadRequest, message)
    { }

    public BadRequestException(string message, Exception innerException)
        : base(HttpStatusCode.BadRequest, message, innerException)
    { }
}
