using DeepSeekChat.Core.Expression;
using DeepSeekChat.Core.Expression.Condition;
using DeepSeekChat.Core.Helpers;
using DeepSeekChat.Core.Network.Body.Attributes;
using DeepSeekChat.Core.Network.Json.Naming;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeepSeekChat.Core.Network.Body;

public record struct RequestParameter(string Name, object Value, ParameterType Type);

public class RequestBody : BodyBase
{
    public virtual IEnumerable<RequestParameter> GetBodyParameters()
    {
        PropertyInfo[] members = GetType().GetProperties();
        List<RequestParameter> result = new();
        foreach (PropertyInfo property in members)
        {
            var memberAttributes = property.GetCustomAttributesData().ToList();

            if (ShouldIgnoreProperty(property, memberAttributes, members))
            {
                continue;
            }

            if (ShouldPinItemsToTopProperty(property, memberAttributes))
            {
                result.AddRange((property.GetValue(this) as RequestBody)?.GetBodyParameters());
                continue;
            }

            string name = GetParameterName(property, memberAttributes);
            ParameterType type = GetParameterType(memberAttributes);
            object value = GetParameterValue(property, memberAttributes);

            result.Add(new RequestParameter(name, value, type));
        }

        return result;
    }

    CustomAttributeData? FindAttribute(Type attributeType, IEnumerable<CustomAttributeData> attributes)
    {
        return attributes.FirstOrDefault(x => x.AttributeType == attributeType);
    }

    List<CustomAttributeData>? FindAttributes(Type attributeType, IEnumerable<CustomAttributeData> attributes)
    {
        return attributes.Where(x => x.AttributeType == attributeType).ToList();
    }

    protected virtual bool ShouldPinItemsToTopProperty(PropertyInfo property, List<CustomAttributeData> attributes)
    {
        return property.PropertyType.IsBasedOn(typeof(RequestBody)) && attributes.Any(attr => attr.AttributeType == typeof(InnerBodyItemsTopmostAttribute));
    }

    protected virtual bool ShouldIgnoreProperty(PropertyInfo property, List<CustomAttributeData> memberAttributes, PropertyInfo[] members)
    {
        var attributeDatas = FindAttributes(typeof(BodyParameterConditionIgnoreAttribute), memberAttributes)
            .Concat(FindAttributes(typeof(BodyParameterModeIgnoreAttribute), memberAttributes));
        if (!attributeDatas.Any())
            return false;

        return attributeDatas.Any(attributeData =>
        {
            return ShouldIgnorePropertyInternalByMode(property, attributeData) ||
                   ShouldIgnorePropertyInternalByCondition(attributeData, members);
        });
    }

    private bool ShouldIgnorePropertyInternalByMode(PropertyInfo property, CustomAttributeData attributeData)
    {
        if (attributeData.AttributeType != typeof(BodyParameterModeIgnoreAttribute))
        {
            return false;
        }

        var ignoreMode = attributeData.ConstructorArguments
            .Cast<CustomAttributeTypedArgument?>()
            .First().Value.Value as int?;
        switch ((IgnoreMode)ignoreMode)
        {
            case IgnoreMode.AlwaysIgnore:
                return true;

            case IgnoreMode.WhenNullIgnore:
                if (property.GetValue(this) == null)
                    return true;
                break;

            case IgnoreMode.WhenStringEmptyIgnore:
                if (property.GetValue(this) is not string str || string.IsNullOrEmpty(str))
                    return true;
                break;

            case IgnoreMode.WhenArrayEmptyIgnore:
                if (property.GetValue(this) is not Array arr || arr.Length == 0)
                    return true;
                break;

            case IgnoreMode.WhenEmptyObjctIgnore:
                var obj = property.GetValue(this);
                if (obj != null)
                {
                    var type = obj.GetType();
                    var hasMembers = type.GetProperties().Length > 0 || type.GetFields().Length > 0;
                    if (!hasMembers)
                        return true;
                }
                break;

            case IgnoreMode.WhenCollectionEmptyIgnore:
                if (property.GetValue(this) is not IEnumerable enumerable || !enumerable.Cast<object>().Any())
                    return true;
                break;
        }

        return false;
    }

    private bool ShouldIgnorePropertyInternalByCondition(CustomAttributeData attributeData, PropertyInfo[] members)
    {
        if(attributeData.AttributeType != typeof(BodyParameterConditionIgnoreAttribute))
        {
            return false;
        }

        string conditionMemberName = attributeData.ConstructorArguments.Cast<CustomAttributeTypedArgument>().First().Value as string;

        if (string.IsNullOrEmpty(conditionMemberName))
            return true;

        FieldInfo fieldInfo = GetType().GetField(conditionMemberName)
            ?? throw new MissingMemberException($"Cannot find name '{conditionMemberName}' for field. Must be only one static field in it.");

        if (fieldInfo.GetValue(this) is not ConditionElement cond)
        {
            throw new NullReferenceException($"Value of field '{conditionMemberName}' is null.");
        }

        foreach (PropertyInfo propertyInternal in members)
        {
            var namedElements = NamedElement.FindNamedElements(cond, propertyInternal.Name);
            foreach (var element in namedElements)
            {
                element.SetValue(propertyInternal.GetValue(this));
            }
        }

        return cond.GetResult();
    }

    protected virtual string GetParameterName(PropertyInfo property, List<CustomAttributeData> memberAttributes)
    {
        var attributeData = FindAttribute(typeof(BodyParameterNameAttribute), memberAttributes);
        if (attributeData != null)
            return attributeData.ConstructorArguments[0].Value as string;

        var namingPolicyAttribute = GetType()
            .GetCustomAttributesData()
            .FirstOrDefault(x => x.AttributeType == typeof(BodyParameterDefaultNamingPolicyAttribute));

        NamingPolicy namingPolicy = namingPolicyAttribute == null
            ? new CamelCaseLower()
            : (NamingPolicy)Activator.CreateInstance(namingPolicyAttribute.ConstructorArguments[0].Value as Type);

        return namingPolicy.GetTransformedName(property.Name);
    }

    protected virtual ParameterType GetParameterType(List<CustomAttributeData> memberAttributes)
    {
        var attributeData = FindAttribute(typeof(BodyParameterTypeAttribute), memberAttributes);
        if (attributeData != null)
            return (ParameterType)attributeData.ConstructorArguments[0].Value;

        var defaultTypeAttribute = GetType()
            .GetCustomAttributesData()
            .FirstOrDefault(x => x.AttributeType == typeof(BodyParameterDefaultTypeAttribute));

        return defaultTypeAttribute == null
            ? ParameterType.RequestBody
            : (ParameterType)defaultTypeAttribute.ConstructorArguments[0].Value;
    }

    protected virtual object GetParameterValue(PropertyInfo property, List<CustomAttributeData> memberAttributes)
    {
        if(FindAttribute(typeof(BodyItemCollectionElementTypeAttribute), memberAttributes) is CustomAttributeData elementTypeAttribute)
            return GetCollectionValue(property, memberAttributes, elementTypeAttribute.ConstructorArguments[0].Value as Type);

        if(FindAttribute(typeof(BodyItemDictionaryValueTypeAttribute), memberAttributes) is CustomAttributeData dictionaryValueTypeAttribute)
            return GetDictionaryValue(property, memberAttributes, dictionaryValueTypeAttribute.ConstructorArguments[0].Value as Type);

        var attributeData = FindAttribute(typeof(BodyItemConverterAttribute), memberAttributes);
        if (attributeData != null)
        {
            var converter = Activator.CreateInstance(attributeData.ConstructorArguments[0].Value as Type) as IBodyParameterValueConverter;
            return converter.ConvertTo(property.GetValue(this));
        }

        var value = property.GetValue(this);

        if (value is not null && value.GetType().IsBasedOn(typeof(RequestBody)))
        {
            return ((RequestBody)value).GetBodyParameters();
        }

        return value;
    }

    protected virtual object GetCollectionValue(PropertyInfo property, List<CustomAttributeData> memberAttributes, Type elementType)
    {
        if (property.GetValue(this) is not object value)
            return new List<object>();
        IList enumerable = Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType), value) as IList;
        var attributeData = FindAttribute(typeof(BodyItemConverterAttribute), memberAttributes);
        IBodyParameterValueConverter converter = default;

        if(elementType.IsBasedOn(typeof(RequestBody)))
        {
            // If the element type is RequestBody, we can handle it differently if needed.
            // For now, we will just use the default converter.
            List<IEnumerable<RequestParameter>> bodyCollections = [];

            foreach (var item in enumerable)
            {
                bodyCollections.Add((item as RequestBody)?.GetBodyParameters() ?? new List<RequestParameter>());
            }

            return bodyCollections;
        }

        List<object> values = [];
        if(attributeData != null)
            converter = Activator.CreateInstance(attributeData.ConstructorArguments[0].Value as Type) as IBodyParameterValueConverter;

        foreach (var item in enumerable)
        {
            values.Add(converter != null? converter.ConvertTo(item) : item);
        }

        return values;
    }

    protected virtual object GetDictionaryValue(PropertyInfo property, List<CustomAttributeData> memberAttributes, Type valueType)
    {
        IDictionary dictionary = Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(typeof(string), valueType), property.GetValue(this)) as IDictionary;
        var attributeData = FindAttribute(typeof(BodyItemConverterAttribute), memberAttributes);
        IBodyParameterValueConverter converter = default;
        
        if(valueType.IsBasedOn(typeof(RequestBody)))
        {
            List<RequestParameter> requestParameters = [];
            foreach (DictionaryEntry item in dictionary)
            {
                requestParameters.Add(new(item.Key.ToString(), (item.Value as RequestBody).GetBodyParameters(),ParameterType.RequestBody));
            }
            return requestParameters;
        }

        Dictionary<string, object> values = [];
        if (attributeData != null)
            converter = Activator.CreateInstance(attributeData.ConstructorArguments[0].Value as Type) as IBodyParameterValueConverter;

        foreach (DictionaryEntry item in dictionary)
        {
            values.Add(item.Key.ToString(), converter != null ? converter.ConvertTo(item.Value) : item.Value);
        }

        return values;
    }

}
