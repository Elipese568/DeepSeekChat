using System;

namespace DeepSeekChat.Core.Network.Body.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class BodyParameterNameAttribute : Attribute
{
    public BodyParameterNameAttribute(string name)
    {
    }
}
