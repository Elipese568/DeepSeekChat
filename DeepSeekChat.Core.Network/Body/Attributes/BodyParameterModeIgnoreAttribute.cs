using System;

namespace DeepSeekChat.Core.Network.Body.Attributes;

public enum IgnoreMode
{
    Unset,
    WhenNullIgnore,
    WhenStringEmptyIgnore,
    WhenArrayEmptyIgnore,
    WhenEmptyObjctIgnore,
    WhenCollectionEmptyIgnore,
    AlwaysIgnore
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class BodyParameterModeIgnoreAttribute : Attribute
{
    public BodyParameterModeIgnoreAttribute(IgnoreMode ignoreMode = IgnoreMode.Unset)
    {
    }
}
