namespace DeepSeekChat.Core.Network.Json.Naming;

/// <summary>
/// Camel-case naming helper
/// </summary>
internal class CamelCaseHelper
{
    private static char ToUpperInternal(char character)
    {
        // Get index of letter in lower letters area
        // (May less than 0)
        int letterIndex = character - 'a';

        // Get upper letter from index of letter
        letterIndex += 'A';

        // Static-cast to Char
        return (char)letterIndex;
    }

    private static char ToLowerInternal(char character)
    {
        // Get index of letter in upper letters area
        // (May less than 0)
        int letterIndex = 'A' - character;

        // Get lower letter from index of letter
        letterIndex = 'a' - letterIndex;

        // Static-cast to Char
        return (char)letterIndex;
    }

    public static char ToUpper(char character)
    {
        // Check this character is lower
        // If true, tp upper
        if (character > 'A')
            return ToUpperInternal(character);

        // If else, return self
        return character;
    }


    public static char ToLower(char character)
    {
        // Check this character is upper
        // If true, to lower
        if (character < 'a')
            return ToLowerInternal(character);

        // If else, return self
        return character;
    }
}
