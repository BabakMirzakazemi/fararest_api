namespace Common.Exceptions;

using System.Net;

public class LogicException : AppException
{
    public LogicException()
        : base(HttpStatusCode.InternalServerError)
    {
    }

    public LogicException(string message)
        : base(HttpStatusCode.InternalServerError, message)
    {
    }

    public LogicException(object additionalData)
        : base(HttpStatusCode.InternalServerError, additionalData)
    {
    }

    public LogicException(string message, object additionalData)
        : base(message, HttpStatusCode.InternalServerError, additionalData)
    {
    }

    public LogicException(string message, Exception exception)
        : base(message, HttpStatusCode.InternalServerError, exception)
    {
    }

    public LogicException(string message, Exception exception, object additionalData)
        : base(message, HttpStatusCode.InternalServerError, exception, additionalData)
    {
    }
}
