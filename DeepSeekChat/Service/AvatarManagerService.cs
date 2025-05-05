using DeepSeekChat.Foundation;
using DeepSeekChat.Models;
using DeepSeekChat.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Service;

[JsonStorageFile(FileName = "avatars.json")]
class AvatarManagerService : JsonSeriailizingServiceBase<AvatarDataStorage>
{
    public const int AVATAR_ID_LENGTH = 24;

    private HeaderAdjustableList<AvatarDataViewModel> _userAvatarDataViewModel;
    private HeaderAdjustableList<AvatarDataViewModel> _aiAvatarDataViewModel;

    protected override void OnInitialize()
    {
        base.OnInitialize();
        _userAvatarDataViewModel = new(_data.UserAvatarIds.Select(x => new AvatarDataViewModel(GetAvatarById(x))));
        _aiAvatarDataViewModel = new(_data.AiAvatarIds.Select(x => new AvatarDataViewModel(GetAvatarById(x))));
    }

    public AvatarManagerService() : base()
    {
    }

    public string GenerateID()
    {
        static char Step()
        {
            int r1 = Random.Shared.Next(0, 52);
            int r2 = Random.Shared.Next(0, 52);
            int s = r1 - r2;
            s = s < 0 ? 0 - s : s;
            return (char)(s > 26 ? s - 26 - 1 + 'a' : s - 1 + 'A');
        }

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < AVATAR_ID_LENGTH; i++)
        {
            sb.Append(Step());
        }

        if (_data.Avatars.Exists(x => x.Id == sb.ToString()))
        {
            return GenerateID();
        }

        return sb.ToString();
    }

    public List<AvatarData> GetStroragedAvatars()
    {
        return _data.Avatars;
    }
    public AvatarData CreateNewAvatar(AvatarType type, string path)
    {
        var avatar = new AvatarData()
        {
            Id = GenerateID(),
            Type = type,
            Path = path,
        };
        _data.Avatars.Add(avatar);
        return avatar;
    }
    public void RemoveAvatar(string id)
    {
        var avatar = _data.Avatars.FirstOrDefault(x => x.Id == id);
        if (avatar != null)
        {
            _data.Avatars.Remove(avatar);
        }
    }
    public AvatarData GetAvatarById(string id)
    {
        return _data.Avatars.FirstOrDefault(x => x.Id == id);
    }
}