using CommunityToolkit.Mvvm.ComponentModel;
using DeepSeekChat.Helper;
using DeepSeekChat.Service;
using DeepSeekChat.Views;
using Microsoft.UI.System;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.ViewModels;

public class SettingViewModel : ObservableRecipient
{
    private readonly SettingService _settingService;
    private readonly ModelsManagerService _modelsManagerService;
    public SettingViewModel()
    {
        _settingService = App.Current.GetService<SettingService>();
        _modelsManagerService = App.Current.GetService<ModelsManagerService>();
        AiModelViewModels = new(_modelsManagerService.GetStroragedModels().Select(x => new AiModelViewModel(x)));
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
        get => Guid.Parse(_settingService.Read(SettingService.SETTING_SELECTED_MODEL, "545B7456-BCF5-4E19-9E23-6C08AD3A90A3"));
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
            SelectedModel = new Guid("F72AB0EC-37D3-43F8-BCC7-A04BBD9B2A37");
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
}
