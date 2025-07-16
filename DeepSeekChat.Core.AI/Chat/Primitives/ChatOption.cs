using DeepSeekChat.Core.AI.Internal.Request;
using DeepSeekChat.Core.Network.Body;
using DeepSeekChat.Core.Network.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Core.AI.Chat.Primitives;

public class ChatOption : IRequestBodyConvertable<ChatCompletionOptionRequestBody>
{
    // Stream property must set by requesting
    public int? MaxTokens { get; set; }
    public bool EnableThinking { get; set; }
    public int? ThinkingBudget { get; set; }
    public double? MinP { get; set; }
    public double? Temperature { get; set; }
    public double? TopP { get; set; }
    public int? TopK { get; set; }
    public double? FrequencyPenalty { get; set; }
    public int? N { get; set; }

    public ChatCompletionOptionRequestBody ToRequestBody()
    {
        return new ChatCompletionOptionRequestBody()
        {
            MaxTokens = MaxTokens,
            EnableThinking = EnableThinking,
            ThinkingBudget = ThinkingBudget,
            MinP = MinP,
            Temperature = Temperature,
            TopP = TopP,
            TopK = TopK,
            FrequencyPenalty = FrequencyPenalty,
            N = N
        };
    }
}
