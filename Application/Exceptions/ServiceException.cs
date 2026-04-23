using System.Net;

namespace Application.Exceptions;

public class ServiceException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public ServiceException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        : base(message)
    {
        StatusCode = statusCode;
    }
}
