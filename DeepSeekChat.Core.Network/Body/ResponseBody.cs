using DeepSeekChat.Core.Network.Body.Attributes;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace DeepSeekChat.Core.Network.Body;

public class ResponseBody : BodyBase
{
    public static async ValueTask<ResponseBodyInfo> MakeResponseBodyAsync<TTarget>(HttpResponseMessage response)
        where TTarget : ResponseBody
    {
        var attributesData = typeof(TTarget).CustomAttributes;

        object dto = new();
        string content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            var errorTypeAttributeData =
                attributesData.Where(x => x.AttributeType == typeof(ResponseErrorTypesAttribute))
                              .Where(x => (int)x.ConstructorArguments[0].Value == (int)response.StatusCode)
                              .FirstOrDefault()
                              ?? throw new InvalidOperationException($"No error type attribute found for status code {(int)response.StatusCode}.");

            var options = BuildSerializerOption(errorTypeAttributeData.ConstructorArguments[1].Value as Type);
            dto = JsonSerializer.Deserialize(content, errorTypeAttributeData.ConstructorArguments[1].Value as Type, options);
        }
        else
        {
            JsonSerializerOptions options = BuildSerializerOption(typeof(TTarget));
            dto = JsonSerializer.Deserialize(content, typeof(TTarget), options);
        }

        return new(content, (int)response.StatusCode, dto);
    }

    static JsonSerializerOptions BuildSerializerOption(Type targetType)
    {
        JsonSerializerOptions options = default;
        if (targetType
            .GetCustomAttributes(true)
            .FirstOrDefault(attr => attr is ResponsePropertyNamingPolicyAttribute, null)
            is ResponsePropertyNamingPolicyAttribute namingPolicyAttr)
        {
            options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = namingPolicyAttr.NamingPolicyMode switch
                {
                    ResponsePropertyNamingMode.CamelCase => JsonNamingPolicy.CamelCase,
                    ResponsePropertyNamingMode.SnakeCaseLower => JsonNamingPolicy.SnakeCaseLower,
                    ResponsePropertyNamingMode.KebabCaseLower => JsonNamingPolicy.KebabCaseLower,
                    ResponsePropertyNamingMode.SnakeCaseUpper => JsonNamingPolicy.SnakeCaseUpper,
                    ResponsePropertyNamingMode.KebabCaseUpper => JsonNamingPolicy.KebabCaseUpper,
                    ResponsePropertyNamingMode.Other => (JsonNamingPolicy)Activator.CreateInstance(namingPolicyAttr.OtherNamingPolicyType)
                },
                PropertyNameCaseInsensitive = true
            };
        }
        else
        {
            options = JsonSerializerOptions.Default;
        }

        return options;
    }
}