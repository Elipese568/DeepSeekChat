using System;

namespace DeepSeekChat.Core.Network.Json.Naming;

public abstract class NamingPolicy
{
    public NamingPolicy() { }

    public virtual string GetTransformedName(string rawName) => throw new InvalidOperationException();
    public virtual string GetUntransformedName(string rawName) => throw new InvalidOperationException();
}
