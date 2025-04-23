using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Models;

public class AiModelStorage
{
    public const string DEEPSEEK_DEFAULT_MODEL_GUID = "545B7456-BCF5-4E19-9E23-6C08AD3A90A3";
    public const string DEEPSEEK_PRO_MODEL_GUID = "F72AB0EC-37D3-43F8-BCC7-A04BBD9B2A37";
    public AiModelStorage()
    {
        AiModels =
        [
            new AiModel()
            {
                Name = "DeepSeek-R1",
                Description = "DeepSeek-R1 是一个基于深度学习的通用大语言推理模型，具有强大的自然语言处理能力。",
                ModelID = "deepseek-ai/DeepSeek-R1",
                UniqueID = new Guid(DEEPSEEK_DEFAULT_MODEL_GUID)
            },
            new AiModel()
            {
                Name = "DeepSeek-R1 (Pro)",
                Description = "DeepSeek-R1 Pro 是一个基于深度学习的通用大语言推理模型，具有强大的自然语言处理能力，且拥有更快的输出速度",
                ModelID = "Pro/deepseek-ai/DeepSeek-R1",
                UniqueID = new Guid(DEEPSEEK_PRO_MODEL_GUID)
            }
        ];
    }
    public List<AiModel> AiModels { get; set; }
    public Guid SelectedModel { get; set; }
}

public class AiModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string ModelID { get; set; }
    public Guid UniqueID { get; set; }
}
