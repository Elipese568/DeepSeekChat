namespace DeepSeekChat.Core.Network.Json.Naming;

public class CamelCaseUpper : CamelCaseBase
{
    public override char GetFirstCharacter(char character, bool reverse)
    {
        return !reverse ? CamelCaseHelper.ToUpper(character) : CamelCaseHelper.ToLower(character);
    }
}
