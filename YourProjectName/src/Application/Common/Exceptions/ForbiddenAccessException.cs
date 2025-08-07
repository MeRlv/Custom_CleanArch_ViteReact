using System.Net;
using YourProjectName.Application.Common.Abstract;

namespace YourProjectName.Application.Common.Exceptions;
public class ForbiddenAccessException : HTTPException
{
    public ForbiddenAccessException()
        : base(HttpStatusCode.Forbidden, "Access is forbidden.")
    { }

    public ForbiddenAccessException(string message)
        : base(HttpStatusCode.Forbidden, message)
    { }

    public ForbiddenAccessException(string message, Exception innerException)
        : base(HttpStatusCode.Forbidden, message, innerException)
    { }
}
