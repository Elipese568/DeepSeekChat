using DeepSeekChat.Core.AI.Internal.Request;
using DeepSeekChat.Core.Network.Body;
using DeepSeekChat.Core.Network.Request;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Core.AI.Chat.Tool;

public abstract class Tool : IRequestBodyConvertable<ToolRequestBody>
{
    public abstract string Name { get; set; }
    public abstract string Description { get; set; }
    public abstract string Type { get; }
    public bool Strict { get; set; }
    public StructuredObjectToolParameter Parameters { get; set; }
    public virtual ToolRequestBody ToRequestBody()
    {
        return new()
        {
            Name = Name,
            Description = Description,
            Type = Type,
            Strict = Strict,
            Parameters = Parameters.ToRequestBody()
        };
    }
}
