namespace DeepSeekChat.Core.AI.Chat.Tool;

public class FunctionTool : Tool
{
    public override string Name { get; set; }
    public override string Description { get; set; }

    public override string Type => "function";
}