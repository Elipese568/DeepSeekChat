using DeepSeekChat.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Models;

[Flags]
public enum AvatarType
{
    User = 0b01,
    Ai = 0b10,
    Default = 0b100
}

public class AvatarData
{
    public string Id { get; set; }
    public AvatarType Type { get; set; }
    public string Path { get; set; }
}

public class AvatarDataStorage
{
    public const string USER_DEFAULT_AVATAR_ID = "RnYADOTYCSMeBfeHmlYVQWfO";
    public const string AI_DEFAULT_AVATAR_ID = "YdaRQHGhBICmgCGIPdZIMFGe";

    public List<AvatarData> Avatars { get; set; }
    public HeaderAdjustableList<string> UserAvatarIds { set; get; }
    public HeaderAdjustableList<string> AiAvatarIds { set; get; }

    public AvatarDataStorage()
    {
        Avatars = new()
        {
            new AvatarData()
            {
                Id = USER_DEFAULT_AVATAR_ID,
                Type = AvatarType.User | AvatarType.Default,
                Path = "ms-appx:///Assets/Avatars/Default/UserDefault.png"
            },
            new AvatarData()
            {
                Id = AI_DEFAULT_AVATAR_ID,
                Type = AvatarType.Ai | AvatarType.Default,
                Path = "ms-appx:///Assets/Avatars/Default/AiDefault.png"
            }
        };
        UserAvatarIds =
        [
            USER_DEFAULT_AVATAR_ID
        ];

        AiAvatarIds =
        [
            AI_DEFAULT_AVATAR_ID
        ];
    }
}
