using CommunityToolkit.WinUI.Controls;
using DeepSeekChat.Foundation;
using DeepSeekChat.Models;
using DeepSeekChat.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;

namespace DeepSeekChat.Service;

public class SelectedAvatarChangedEventArgs : EventArgs
{
    public AvatarType Type { get; set; }
    public AvatarDataViewModel ViewModel { get; set; }
}

public interface IAvatarEnabler
{
    public StorageFile RawFile { get; }
    public AvatarData Enable();
}

[JsonStorageFile(FileName = "avatars.json")]
public class AvatarManagerService : JsonSeriailizingServiceBase<AvatarDataStorage>
{
    private class __AvatarEnabler : IAvatarEnabler
    {
        private AvatarData _data;
        private Action<AvatarData> _enableAction;
        public __AvatarEnabler(AvatarData data, Action<AvatarData> enableAction)
        {
            _data = data;
            _enableAction = enableAction;
            RawFile = ApplicationData.Current.LocalFolder.CreateFileAsync(Path.GetFileName(data.Path), CreationCollisionOption.OpenIfExists).GetAwaiter().GetResult();
        }
        public StorageFile RawFile { get; private set; }
        public AvatarData Enable()
        {
            _enableAction(_data);
            return _data;
        }
    }
    public const int AVATAR_ID_LENGTH = 24;

    private HeaderAdjustableList<AvatarDataViewModel> _userAvatarDataViewModel;
    private HeaderAdjustableList<AvatarDataViewModel> _aiAvatarDataViewModel;

    private EventHandlerWrapper<EventHandler<SelectedAvatarChangedEventArgs>> _selectedUserAvatarChanged;
    public event EventHandler<SelectedAvatarChangedEventArgs> SelectedUserAvatarChanged
    {
        add
        {
            _selectedUserAvatarChanged.AddHandler(value);
        }
        remove
        {
            _selectedUserAvatarChanged.RemoveHandler(value);
        }
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();
        _userAvatarDataViewModel = new(_data.UserAvatarIds.Select(x => new AvatarDataViewModel(GetAvatarById(x))));
        _aiAvatarDataViewModel = new(_data.AiAvatarIds.Select(x => new AvatarDataViewModel(GetAvatarById(x))));
        _selectedUserAvatarChanged = EventHandlerWrapper<EventHandler<SelectedAvatarChangedEventArgs>>.Create();
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
            return (char)(s >= 26 ? s - 26 - 1 + 'a' : s - 1 + 'A');
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
    public IAvatarEnabler CreateNewAvatar(AvatarType type, bool setNew = true)
    {
        var avatar = new AvatarData()
        {
            Id = GenerateID(),
            Type = type,
            Path = Path.Combine(ApplicationData.Current.LocalFolder.Path, GenerateID() + ".png")
        };
        _data.Avatars.Add(avatar);
        return new __AvatarEnabler(avatar, (avatar) =>
        {
            if (type.HasFlag(AvatarType.User))
            {
                _data.UserAvatarIds.Add(avatar.Id);
                _userAvatarDataViewModel.Add(new AvatarDataViewModel(avatar));
            }
            else if (type.HasFlag(AvatarType.Ai))
            {
                _data.AiAvatarIds.Add(avatar.Id);
                _aiAvatarDataViewModel.Add(new AvatarDataViewModel(avatar));
            }
            else
            {
                throw new ArgumentException("Invalid AvatarType");
            }
            _selectedUserAvatarChanged.Invoke(this, new SelectedAvatarChangedEventArgs()
            {
                Type = avatar.Type,
                ViewModel = type.HasFlag(AvatarType.User) ? _userAvatarDataViewModel[0] : _aiAvatarDataViewModel[0]
            });
        });
        
    }

    public void RemoveAvatar(string id)
    {
        var avatar = _data.Avatars.FirstOrDefault(x => x.Id == id);
        if (avatar != null)
        {
            _data.Avatars.Remove(avatar);
        }
    }
    public AvatarDataViewModel GetSelectedUserAvatarViewModel()
    {
        return _userAvatarDataViewModel[0];
    }
    public AvatarDataViewModel GetSelectedAiAvatarViewModel()
    {
        return _aiAvatarDataViewModel[0];
    }

    public void ResetAvatar(AvatarType type)
    {
        if(type.HasFlag(AvatarType.User))
        {
            _data.UserAvatarIds.RaiseToHead(AvatarDataStorage.USER_DEFAULT_AVATAR_ID);
            _userAvatarDataViewModel.RaiseToHead(_userAvatarDataViewModel.First(x => x.Id == AvatarDataStorage.USER_DEFAULT_AVATAR_ID));
        }
        else if(type.HasFlag(AvatarType.Ai))
        {
            _data.AiAvatarIds.RaiseToHead(AvatarDataStorage.AI_DEFAULT_AVATAR_ID);
            _aiAvatarDataViewModel.RaiseToHead(_aiAvatarDataViewModel.First(x => x.Id == AvatarDataStorage.AI_DEFAULT_AVATAR_ID));
        }

        _selectedUserAvatarChanged.Invoke(this, new SelectedAvatarChangedEventArgs()
        {
            Type = type,
            ViewModel = type.HasFlag(AvatarType.User) ? _userAvatarDataViewModel[0] : _aiAvatarDataViewModel[0]
        });
    }

    public AvatarData GetAvatarById(string id)
    {
        return _data.Avatars.FirstOrDefault(x => x.Id == id);
    }
}