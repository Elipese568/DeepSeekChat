using System;

namespace DeepSeekChat.Core.Network.Body.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class BodyParameterDefaultTypeAttribute : Attribute
{
    public BodyParameterDefaultTypeAttribute(ParameterType parameterType) { }
}
