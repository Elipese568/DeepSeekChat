using System;

namespace DeepSeekChat.Core.Network.Body.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class BodyParameterTypeAttribute : Attribute
{
    public BodyParameterTypeAttribute(ParameterType parameterType)
    { }
}
