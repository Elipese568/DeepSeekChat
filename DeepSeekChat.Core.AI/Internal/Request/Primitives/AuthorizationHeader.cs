namespace DeepSeekChat.Core.AI.Internal.Request.Primitives;

public readonly struct AuthorizationHeader
{
    public readonly string ApiKey { get; }

    public AuthorizationHeader(string token)
    {
        ApiKey = token;
    }
}
