using System;
using System.Net;

namespace YourProjectName.Application.Common.Abstract
{
  public abstract class HTTPException : Exception
  {
    public HttpStatusCode StatusCode { get; }

    protected HTTPException(HttpStatusCode statusCode)
      : base()
    {
      StatusCode = statusCode;
    }

    protected HTTPException(HttpStatusCode statusCode, string message)
      : base(message)
    {
      StatusCode = statusCode;
    }

    protected HTTPException(HttpStatusCode statusCode, string message, Exception innerException)
      : base(message, innerException)
    {
      StatusCode = statusCode;
    }
  }
}