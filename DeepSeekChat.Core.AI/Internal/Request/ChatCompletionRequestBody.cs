using DeepSeekChat.Core.AI.Internal.Request.Primitives;
using DeepSeekChat.Core.Network.Body;
using DeepSeekChat.Core.Network.Body.Attributes;

namespace DeepSeekChat.Core.AI.Internal.Request;

//var request = new ChatCompletionRequestBody()
//{
//    AuthorizationHeader = new AuthorizationHeader(GetService<SettingService>().Read(SettingService.SETTING_APIKEY, "")),
//    FrequencyPenalty = 0,
//    EnableThinking = true,
//    MaxTokens = 114514,
//    Messages = [],
//    MinP = 0,
//    Model = "deepseek",
//    N = 1,
//    ResponseFormat = new ResponseFormatRequestBody() { Type = "text" },
//    Stop = ["##"],
//    Stream = true,
//    Temperature = 1.0,
//    ThinkingBudget = 1000,
//    Tools =
//                [
//                    new ToolRequestBody (){
//                        Type = "function",
//                        Name = "test",
//                        Description = "test function",
//                        Parameters = new FunctionCallingProperties(){
//                            Description = "test function parameters",
//                            Type = ["object"],
//                            Properties = new Dictionary<string, FunctionCallingProperties>(){
//                                ["a"] = new FunctionCallingProperties(){
//                                    Type = ["int"],
//                                    Description = "add number",
//                                },
//                                ["b"] = new FunctionCallingProperties(){
//                                    Type = ["int"],
//                                    Description = "add number",
//                                },
//                                ["c"] = new FunctionCallingProperties(){
//                                    Type = ["object"],
//                                    Description = "test structure",
//                                    Properties = new Dictionary<string, FunctionCallingProperties>(){
//                                        ["c_a"] = new(){
//                                            Type = ["int"],
//                                            Description = "c_a number",
//                                        },
//                                        ["c_b"] = new FunctionCallingProperties(){
//                                            Type = ["string"],
//                                            Description = "c_b mode",
//                                            ExtraParameters = new Dictionary<string, object>(){
//                                                ["enum"] = new[]{
//                                                    "a",
//                                                    "b",
//                                                    "c",
//                                                }
//                                            }
//                                        }
//                                    },
//                                    Required = ["c_a", "c_b"],
//                                    AdditionalProperties = false,
//                                },
//                            },
//                            Required = ["a", "b"],
//                            AdditionalProperties = false,
//                        },
//                        Strict = true
//                    },
//                    new ToolRequestBody(){
//                       Type = "codeInspection",
//                       Name = "Ignore Test",
//                       Description = "This is a test tool to ignore parameters",
//                       Strict = false
//                    }
//                ],
//    TopK = 50,
//    TopP = 0.95,
//};

public class ChatCompletionRequestBody : AuthorizationOperationBodyBase
{
    public string Model { get; set; }

    [InnerBodyItemsTopmost]
    public ChatCompletionOptionRequestBody Options { get; set; }

    [BodyItemCollectionElementType(typeof(MessageRequestBody))]
    public MessageRequestBody[] Messages { get; set; }

    [BodyItemCollectionElementType(typeof(ToolRequestBody))]
    [BodyParameterModeIgnore(IgnoreMode.WhenCollectionEmptyIgnore)]
    public ToolRequestBody[] Tools { get; set; }
}

public class ChatCompletionOptionRequestBody : RequestBody
{
    public bool Stream { get; set; }
    public int? MaxTokens { get; set; }
    public bool EnableThinking { get; set; }
    public int? ThinkingBudget { get; set; }
    public double? MinP { get; set; }
    public double? Temperature { get; set; }
    public double? TopP { get; set; }
    public int? TopK { get; set; }
    public double? FrequencyPenalty { get; set; }
    public int? N { get; set; }

    public ResponseFormatRequestBody ResponseFormat { get; set; }

    [BodyItemConverter(typeof(MultipleValueParameterConverter<string>))]
    [BodyParameterModeIgnore(IgnoreMode.WhenCollectionEmptyIgnore)]
    public MultipleValue<string> Stop { get; set; }
}
