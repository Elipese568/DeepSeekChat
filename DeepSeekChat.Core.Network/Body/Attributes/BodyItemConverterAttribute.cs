using System;
using System.Linq;

namespace DeepSeekChat.Core.Network.Body.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class BodyItemConverterAttribute : Attribute
{
    public BodyItemConverterAttribute(Type converterType)
    {
        if (!converterType.GetInterfaces().Contains(typeof(IBodyParameterValueConverter)))
        {
            throw new ArgumentException($"Param '{nameof(converterType)}' isn't based on interface {typeof(IBodyParameterValueConverter).FullName}.");
        }
    }
}
