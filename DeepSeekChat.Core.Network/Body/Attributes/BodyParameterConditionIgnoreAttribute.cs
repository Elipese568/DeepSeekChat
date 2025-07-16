using System;

namespace DeepSeekChat.Core.Network.Body.Attributes;
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class BodyParameterConditionIgnoreAttribute : Attribute
{
    public BodyParameterConditionIgnoreAttribute(string conditionMemberName)
    {
    }
}
