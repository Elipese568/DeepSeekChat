using DeepSeekChat.Models;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.ViewModels;

public class AvatarDataViewModel : WrapperViewModelBase<AvatarData>
{
    public AvatarDataViewModel(AvatarData avatarData) : base(avatarData)
    {
        _innerObject = avatarData;
        ImageSource = new BitmapImage(new Uri(avatarData.Path));
    }

    public string Id
    {
        get => _innerObject.Id;
    }

    public AvatarType Type
    {
        get => _innerObject.Type;
    }

    public string Path
    {
        get => _innerObject.Path;
    }

    public BitmapSource ImageSource { get; set; }

    private static Dictionary<string, AvatarDataViewModel> _avatarDataViewModelsCache = new();
}
