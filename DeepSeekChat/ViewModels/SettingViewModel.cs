using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.Controls;
using DeepSeekChat.Helper;
using DeepSeekChat.Models;
using DeepSeekChat.Service;
using DeepSeekChat.Views;
using Microsoft.UI.System;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage.Pickers.Provider;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;

namespace DeepSeekChat.ViewModels;

public partial class SettingViewModel : ObservableRecipient
{
    private readonly SettingService _settingService;
    private readonly ModelsManagerService _modelsManagerService;
    private readonly AvatarManagerService _avatarManagerService;
    private readonly SettingPage _settingPage;

    [ObservableProperty]
    private AvatarDataViewModel _selectedUserAvatar;

    [ObservableProperty]
    private AvatarDataViewModel _selectedAiAvatar;
    public SettingViewModel(SettingPage page)
    {
        _settingService = App.Current.GetService<SettingService>();
        _modelsManagerService = App.Current.GetService<ModelsManagerService>();
        _avatarManagerService = App.Current.GetService<AvatarManagerService>();
        AiModelViewModels = new(_modelsManagerService.GetStroragedModels().Select(x => new AiModelViewModel(x)));

        SelectedUserAvatar = _avatarManagerService.GetSelectedUserAvatarViewModel();
        SelectedAiAvatar = _avatarManagerService.GetSelectedAiAvatarViewModel();
        _settingPage = page;
    }

    public string ApiKey
    {
        get => _settingService.Read(SettingService.SETTING_APIKEY, string.Empty);
        set
        {
            _settingService.Write(SettingService.SETTING_APIKEY, value);
            OnPropertyChanged();
        }
    }

    public int ApplicationTheme
    {
        get => int.Parse(_settingService.Read(SettingService.SETTING_THEME, "0"));
        set
        {
            _settingService.Write(SettingService.SETTING_THEME, value.ToString());
            ((FrameworkElement)MainWindow.Current.Content).RequestedTheme = (ElementTheme)value;
            OnPropertyChanged();
        }
    }

    public Guid SelectedModel
    {
        get => Guid.Parse(_settingService.Read(SettingService.SETTING_SELECTED_MODEL, AiModelStorage.DEEPSEEK_DEFAULT_MODEL_GUID));
        set
        {
            _settingService.Write(SettingService.SETTING_SELECTED_MODEL, value.ToString());
            OnPropertyChanged();
        }
    }

    public ObservableCollection<AiModelViewModel> AiModelViewModels { get; set; }

    public void RemoveModel(Guid modelUniqueID)
    {
        int index = AiModelViewModels.IndexOf(x => x.UniqueID == modelUniqueID);
        bool isBack = index == AiModelViewModels.Count - 1;

        _modelsManagerService.RemoveModel(modelUniqueID);
        AiModelViewModels.RemoveAt(index);
        if (!(AiModelViewModels.Count == 0))
            if (isBack)
                SelectedModel = AiModelViewModels[index - 1].UniqueID;
            else
                SelectedModel = AiModelViewModels[index].UniqueID;
        else
            SelectedModel = new Guid(AiModelStorage.DEEPSEEK_PRO_MODEL_GUID);
    }

    public void AddModel(string name, string description, string modelID)
    {
        var model = _modelsManagerService.CreateNewModel(name, description, modelID);
        AiModelViewModels.Add(new(model));
    }

    public AiModelViewModel GetModelViewModel(Guid modelUniqueID)
    {
        return AiModelViewModels.FirstOrDefault(x => x.UniqueID == modelUniqueID);
    }

    [RelayCommand]
    public async void SelectAvatar(int type)
    {
        FileOpenPicker picker = new();
        var window = MainWindow.Current;

        WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(window));

        picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
        picker.FileTypeFilter.Add(".png");
        picker.FileTypeFilter.Add(".jpg");
        picker.FileTypeFilter.Add(".jpeg");
        picker.ViewMode = PickerViewMode.Thumbnail;

        var ava = await picker.PickSingleFileAsync();

        var imageCropper = new ImageCropper
        {
            Width = 600,
            Height = 600,
            CropShape = CropShape.Circular,
            AspectRatio = 1 / 1,
            ThumbPlacement = ThumbPlacement.All,
            VerticalContentAlignment = VerticalAlignment.Top,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            Padding = new(0, 0, 100, 0),
            Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0))
        };

        await imageCropper.LoadImageFromFile(ava);
        var result = await ContentDialogHelper.ShowContentDialog("Selected Avatar", imageCropper, "OK", "Cancel", null, ContentDialogButton.Primary, _settingPage.XamlRoot);
        if(result != ContentDialogResult.Primary)
            return;

        var enabler = _avatarManagerService.CreateNewAvatar((AvatarType)type, true);
        await imageCropper.SaveAsync(await enabler.RawFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite), BitmapFileFormat.Png, true);
        enabler.Enable();

        if(type == (int)AvatarType.User)
        {
            SelectedUserAvatar = _avatarManagerService.GetSelectedUserAvatarViewModel();
        }
        else if (type == (int)AvatarType.Ai)
        {
            SelectedAiAvatar = _avatarManagerService.GetSelectedAiAvatarViewModel();
        }
    }

    [RelayCommand]
    public void ResetAvatar(int type)
    {
        _avatarManagerService.ResetAvatar((AvatarType)type);
        if (type == (int)AvatarType.User)
        {
            SelectedUserAvatar = _avatarManagerService.GetSelectedUserAvatarViewModel();
        }
        else if (type == (int)AvatarType.Ai)
        {
            SelectedAiAvatar = _avatarManagerService.GetSelectedAiAvatarViewModel();
        }
    }
}
