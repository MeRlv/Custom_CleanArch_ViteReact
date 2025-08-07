using System.Net;
using YourProjectName.Application.Common.Abstract;

namespace YourProjectName.Application.Common.Exceptions;

public class UnauthorizedException : HTTPException
{
    public UnauthorizedException()
        : base(HttpStatusCode.Unauthorized, "Unauthorized access.")
    { }

    public UnauthorizedException(string message)
        : base(HttpStatusCode.Unauthorized, message)
    { }

    public UnauthorizedException(string message, Exception innerException)
        : base(HttpStatusCode.Unauthorized, message, innerException)
    { }
}
