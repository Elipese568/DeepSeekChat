using DeepSeekChat.Core.Expression;
using DeepSeekChat.Core.Expression.Condition;
using DeepSeekChat.Core.Expression.Condition.Multi;
using DeepSeekChat.Core.Helpers;
using DeepSeekChat.Core.Network.Body;
using DeepSeekChat.Core.Network.Body.Attributes;
using DeepSeekChat.Core.Network.Json.Naming;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace DeepSeekChat.Core.AI.Internal.Request.Primitives;

[BodyParameterDefaultType(ParameterType.RequestBody)]
[BodyParameterDefaultNamingPolicy(typeof(CamelCaseLower))]
public class JsonContentTypeRequestBodyBase : RequestBody
{
    static JsonContentTypeRequestBodyBase()
    {

    }

    private static readonly ConditionDictionary<Action<JsonContentTypeRequestBodyBase,Utf8JsonWriter, dynamic>> WriteMethodsDict = new()
    {
        [NumberTypeMatchCondition.Equal.ToDictionaryKey()] = (self, w, v) => w.WriteNumberValue(v),
        [CharacterTypeMatchCondition.Equal.ToDictionaryKey()] = (self, w, v) => w.WriteStringValue(v),
        [new TypeMatchCondition(ConditionDictionary.KeyElement, ValueElement.MakeValueElement(typeof(bool)))] = (self, w, v) => w.WriteBooleanValue(v),
        [new ArrayTypeCondition(ConditionDictionary.KeyElement)] = (self, w, v) =>
        {
            if (v is IEnumerable<RequestParameter> parameters)
            {
                w.WriteStartObject();
                self.WriteParameters(parameters, w);
                w.WriteEndObject();
            }
            else if (v is IEnumerable enumer && enumer.Cast<object>().All(item => item is IEnumerable<RequestParameter>))
            {
                w.WriteStartArray();
                foreach (var parameterCollection in enumer.Cast<IEnumerable<RequestParameter>>())
                {
                    w.WriteStartObject();
                    self.WriteParameters(parameterCollection, w);
                    w.WriteEndObject();
                }
                w.WriteEndArray();
            }
            else
            {
                w.WriteStartArray();
                foreach (var item in v)
                {
                    if (item is null)
                    {
                        w.WriteNullValue();
                    }
                    else
                    {
                        WriteMethodsDict.GetValue(item.GetType())(self,w, item);
                    }
                }
                w.WriteEndArray();
            }
        },
        [new TypeMatchCondition(ConditionDictionary.KeyElement, ValueElement.MakeValueElement(typeof(RequestBody)), true)] = (self, w, v) =>
        {
            if (v is null)
            {
                w.WriteNullValue();
                return;
            }
            w.WriteStartObject();
            self.WriteParameters(((RequestBody)v).GetBodyParameters(), w);
            w.WriteEndObject();
        },
        [new TrueCondition()] = (self, w, v) =>
        {
            if (v is null)
            {
                w.WriteNullValue();
            }
            JsonSerializer.Serialize(w, v);
        }
    };
    private void WriteParameters(IEnumerable<RequestParameter> parameters, Utf8JsonWriter writer)
    {
        foreach (var param in parameters)
        {
            writer.WritePropertyName(param.Name);
            var writeMethod = WriteMethodsDict.GetValue(param.Value.GetType());
            writeMethod(this,writer, param.Value);
        }
    }

    public virtual IEnumerable<RequestParameter> GetRawRequestParameters()
    {
        return base.GetBodyParameters().ToList();
    }

    public override IEnumerable<RequestParameter> GetBodyParameters()
    {
        var parameters = GetRawRequestParameters();

        var rawStream = new MemoryStream();
        Utf8JsonWriter writer = new(rawStream, new JsonWriterOptions { Indented = true });
        writer.WriteStartObject();

        var shouldBeIgroned = parameters.Where(p => p.Type != ParameterType.RequestBody).ToList();
        var needProc = parameters.Except(shouldBeIgroned);
        WriteParameters(needProc, writer);

        writer.WriteEndObject();
        writer.Flush();

        rawStream.Seek(0, SeekOrigin.Begin);
        return [new("Content", Encoding.UTF8.GetString(rawStream.GetBuffer()), ParameterType.RequestBody), .. shouldBeIgroned];
    }
}
