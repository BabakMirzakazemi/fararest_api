namespace Common.Exceptions;

using System.Net;
public class NotFoundException : AppException
{
    public NotFoundException()
        : base(HttpStatusCode.NotFound)
    {
    }

    public NotFoundException(string message)
        : base(message, HttpStatusCode.NotFound)
    {
    }

    public NotFoundException(object additionalData)
        : base(null, HttpStatusCode.NotFound, additionalData)
    {
    }

    public NotFoundException(string message, object additionalData)
        : base(message, HttpStatusCode.NotFound, additionalData)
    {
    }

    public NotFoundException(string message, Exception exception)
        : base(message, HttpStatusCode.NotFound, exception)
    {
    }

    public NotFoundException(string message, Exception exception, object additionalData)
        : base(message, HttpStatusCode.NotFound, exception, additionalData)
    {
    }
}
