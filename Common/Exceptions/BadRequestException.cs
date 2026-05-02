namespace Common.Exceptions;

using System.Net;

public class BadRequestException : AppException
{
    public BadRequestException()
        : base(HttpStatusCode.BadRequest)
    {
    }

    public BadRequestException(string message)
        : base(message, HttpStatusCode.BadRequest)
    {
    }

    public BadRequestException(object additionalData)
        : base(null, HttpStatusCode.BadRequest, additionalData)
    {
    }

    public BadRequestException(string message, object additionalData)
        : base(message, HttpStatusCode.BadRequest, additionalData)
    {
    }

    public BadRequestException(string message, Exception exception)
        : base(message, HttpStatusCode.BadRequest, exception)
    {
    }

    public BadRequestException(string message, Exception exception, object additionalData)
        : base(message, HttpStatusCode.BadRequest, exception, additionalData)
    {
    }
}
