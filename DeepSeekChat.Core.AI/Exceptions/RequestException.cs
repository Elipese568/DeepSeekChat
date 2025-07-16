using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Core.AI.Exceptions;

public class RequestException : Exception
{
    public int StatusCode { get; }

    public RequestException(string message, int statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }
}

public class RateLimitExceededException : RequestException
{
    public RateLimitExceededException(string message)
        : base(message, 429) { }
}

public class TokenLimitExceededException : RequestException
{
    public TokenLimitExceededException(string message)
        : base(message, 400) { }
}

public class RequestTooLargeException : RequestException
{
    public RequestTooLargeException(string message)
        : base(message, 413) { }
}

public class ServiceOverloadException : RequestException
{
    public ServiceOverloadException(string message)
        : base(message, 503) { }
}
