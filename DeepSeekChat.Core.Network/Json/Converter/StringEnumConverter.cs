using DeepSeekChat.Core.Network.Body;
using DeepSeekChat.Core.Network.Json.Naming;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeepSeekChat.Core.Network.Json.Converter;

/// <summary>
/// Converter for string to enum or from enum.
/// </summary>
/// <typeparam name="TEnum">Enum type.</typeparam>
/// <typeparam name="TNamingPolicy">Naming policy.</typeparam>
public class StringEnumConverter<TEnum, TNamingPolicy>
    where TNamingPolicy : NamingPolicy
    where TEnum : struct, Enum
{
    public TEnum ToEnum(string itemName)
    {
        if (Enum.TryParse(typeof(TEnum), itemName, out object item))
        {
            return (TEnum)item;
        }
        return default;
    }

    public string ToValueString(TEnum value)
    {
        string name = Enum.GetName(typeof(TEnum), value);
        TNamingPolicy namingPolicy = Activator.CreateInstance(typeof(TNamingPolicy)) as TNamingPolicy;

        return namingPolicy.GetTransformedName(Enum.GetName(value));
    }
}

public class JsonStringEnumConverter<TEnum, TNamingPolicy> : JsonConverter<TEnum>
    where TNamingPolicy : NamingPolicy
    where TEnum : struct, Enum
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string itemName = reader.GetString();
        return
            new StringEnumConverter<TEnum, TNamingPolicy>()
                .ToEnum(itemName);

    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(
            new StringEnumConverter<TEnum, TNamingPolicy>()
                .ToValueString(value)
            );
    }
}

public class BodyParameterStringEnumConverter<TEnum, TNamingPolicy> : IBodyParameterValueConverter
    where TNamingPolicy : NamingPolicy
    where TEnum : struct, Enum
{
    private StringEnumConverter<TEnum, TNamingPolicy> _innerConverter = new();

    public object ConvertTo(object value)
    {
        return _innerConverter.ToValueString((TEnum)value);
    }

    public object ConvertBack(object value, Type targetType)
    {
        return _innerConverter.ToEnum((string)value);
    }
}
