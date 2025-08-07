using System.Net;
using YourProjectName.Application.Common.Abstract;

namespace YourProjectName.Application.Common.Exceptions;
public class NotFoundException : HTTPException
{
    public NotFoundException()
        : base(HttpStatusCode.NotFound, "Resource not found.")
    { }

    public NotFoundException(string message)
        : base(HttpStatusCode.NotFound, message)
    { }

    public NotFoundException(string message, Exception innerException)
        : base(HttpStatusCode.NotFound, message, innerException)
    { }
}
