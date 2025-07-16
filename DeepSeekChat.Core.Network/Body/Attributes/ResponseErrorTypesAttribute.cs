using DeepSeekChat.Core.Helpers;
using DeepSeekChat.Core.Network.Model.Errors;
using System;

namespace DeepSeekChat.Core.Network.Body.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ResponseErrorTypesAttribute : Attribute
{
#pragma warning disable
    public ResponseErrorTypesAttribute(int statusCode, Type type)
    {
        if (!(type.BaseType.IsBasedOn(typeof(ResponseError))))
            throw new ArgumentException($"Error-type `{type.FullName}` must be based on `{typeof(ResponseError).FullName}`.");
    }
}
