using DeepSeekChat.Core.Expression;
using DeepSeekChat.Core.Expression.Condition;
using DeepSeekChat.Core.Expression.Condition.Multi;
using System;

namespace DeepSeekChat.Core.AI.Chat.Tool;

public class ValueToolParameter<T> : ToolParameter
{
    private string _type;
    private static ConditionDictionary<string> _typeNameTable = new()
    {
        [new TypeMatchCondition(ConditionDictionary.KeyElement, ValueElement.MakeValueElement(typeof(int)))] = "int32",
        [new TypeMatchCondition(ConditionDictionary.KeyElement, ValueElement.MakeValueElement(typeof(long)))] = "int64",
        [new TypeMatchCondition(ConditionDictionary.KeyElement, ValueElement.MakeValueElement(typeof(uint)))] = "unsigned_int32",
        [new TypeMatchCondition(ConditionDictionary.KeyElement, ValueElement.MakeValueElement(typeof(ulong)))] = "unsigned_int64",
        [new TypeMatchCondition(ConditionDictionary.KeyElement, ValueElement.MakeValueElement(typeof(nint)))] = Environment.Is64BitOperatingSystem ? "int64" : "int32",
        [new TypeMatchCondition(ConditionDictionary.KeyElement, ValueElement.MakeValueElement(typeof(nuint)))] = Environment.Is64BitOperatingSystem ? "uint64" : "uint32",
        [new TypeMatchCondition(ConditionDictionary.KeyElement, ValueElement.MakeValueElement(typeof(short)))] = "int16",
        [new TypeMatchCondition(ConditionDictionary.KeyElement, ValueElement.MakeValueElement(typeof(ushort)))] = "uint16",
        [new TypeMatchCondition(ConditionDictionary.KeyElement, ValueElement.MakeValueElement(typeof(byte)))] = "uint8",
        [new TypeMatchCondition(ConditionDictionary.KeyElement, ValueElement.MakeValueElement(typeof(sbyte)))] = "int8",
        [new TypeMatchCondition(ConditionDictionary.KeyElement, ValueElement.MakeValueElement(typeof(char)))] = "char",
        [new TypeMatchCondition(ConditionDictionary.KeyElement, ValueElement.MakeValueElement(typeof(string)))] = "string",
        [new TypeMatchCondition(ConditionDictionary.KeyElement, ValueElement.MakeValueElement(typeof(bool)))] = "bool",
        [new TypeMatchCondition(ConditionDictionary.KeyElement, ValueElement.MakeValueElement(typeof(nuint)))]
    };
    public ValueToolParameter()
    {
        _type = _typeNameTable[typeof(T)][0];
    }

    public bool IsNullable { get; set; }

    public override string[] Type => IsNullable? [_type, "null"] : [_type];
}
