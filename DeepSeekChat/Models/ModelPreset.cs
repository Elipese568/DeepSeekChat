using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Models;

public class ModelPreset
{
    public Uri IconPath { get; set; }
    public string ProviderName { get; set; }

    public List<AiModel> models { get; set; }
}
