using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DeepSeekChat.Core.Helpers;

public static class TypeHelper
{
    public struct BaseTypeLinkEnumerable : IEnumerable<Type>
    {
        private readonly Type _type;
        private readonly bool _isSelfContainedLink;

        public BaseTypeLinkEnumerable(Type type, bool isSelfContainedLink)
        {
            _type = type;
            _isSelfContainedLink = isSelfContainedLink;
        }

        public IEnumerator<Type> GetEnumerator()
        {
            return new BaseTypeLinkEnumerator(_type, _isSelfContainedLink);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new BaseTypeLinkEnumerator(_type, _isSelfContainedLink);
        }
    }
    public struct BaseTypeLinkEnumerator : IEnumerator<Type>
    {
        private Type? m_current = default;
        private readonly Type Object_TypeInfo = typeof(object);
        private readonly Type m_start;
        private bool m_isSelfContainedLink;
        private bool m_isStart = true;

        public BaseTypeLinkEnumerator(Type t, bool hasSelf = false)
        {
            m_current = t;
            m_start = t;
            m_isSelfContainedLink = hasSelf;
        }

        public Type Current => m_current;

        object IEnumerator.Current => m_current;

        public void Dispose() { }

        public bool MoveNext()
        {
            if (m_current == Object_TypeInfo)
            {
                return false;
            }

            if (m_isSelfContainedLink && m_isStart)
            {
                m_isStart = false;
                return true;
            }

            m_current = m_current.BaseType;
            return true;
        }

        public void Reset()
        {
            m_current = m_start;
            if (m_isSelfContainedLink)
                m_isStart = true;
        }
    }
    public static IEnumerable<Type> GetBasedLinkTypesWithSelf(this Type type)
    {
        return new BaseTypeLinkEnumerable(type, isSelfContainedLink: true);
    }
    public static IEnumerable<Type> GetBasedLinkTypesWithoutSelf(this Type type)
    {
        return new BaseTypeLinkEnumerable(type, isSelfContainedLink: false);
    }

    public static IEnumerable<Type> GetBasedLinkTypeInterfacesWithSelf(this Type type)
    {
        yield return type;
        foreach (var basedType in GetBasedLinkTypeInterfaceInternal(type))
        {
            yield return basedType;
        }
    }

    public static IEnumerable<Type> GetBasedLinkTypeInterfacesWithoutSelf(this Type type)
    {
        return GetBasedLinkTypeInterfaceInternal(type).ToList();
    }

    private static IEnumerable<Type> GetBasedLinkTypeInterfaceInternal(Type type)
    {
        List<Type> types = [..type.GetInterfaces()];
        foreach(var basedType in type.GetBasedLinkTypesWithoutSelf())
        {
            types.Add(basedType);
            types.AddRange(basedType.GetBasedLinkTypeInterfacesWithoutSelf());
        }
        if (type == typeof(object))
            return types;

        return types;
    }

    public static bool IsBasedOn(this Type type, Type baseType)
    {
        return GetBasedLinkTypeInterfaceInternal(type).Contains(baseType) || type == baseType;
    }
}
