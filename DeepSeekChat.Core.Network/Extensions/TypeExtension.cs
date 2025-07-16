using System;
using System.Collections.Generic;

namespace DeepSeekChat.Core.Network.Extensions;

public static class TypeExtension
{
    public static bool ContainType(this ICollection<Type> typeCollection, Type type)
    {
        foreach (var item in typeCollection)
        {
            if (item.Equals(type))
                return true;
        }
        return false;
    }
}
