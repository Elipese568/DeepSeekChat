namespace DeepSeekChat.Core.Network.Json.Naming;

public class CamelCaseLower : CamelCaseBase
{
    public override char GetFirstCharacter(char character, bool reverse)
    {
        return !reverse ? CamelCaseHelper.ToLower(character) : CamelCaseHelper.ToUpper(character);
    }
}
