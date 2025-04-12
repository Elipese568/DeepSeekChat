using CommunityToolkit.Mvvm.ComponentModel;
using DeepSeekChat.Helper;
using DeepSeekChat.Views;
using Microsoft.UI.System;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.ViewModels;

public class SettingViewModel : ObservableRecipient
{
    public string ApiKey
    {
        get => SettingHelper.Read(nameof(ApiKey), string.Empty);
        set
        {
            SettingHelper.Write(nameof(ApiKey), value);
            OnPropertyChanged();
        }
    }

    public int ApplicationTheme
    {
        get => int.Parse(SettingHelper.Read(nameof(ApplicationTheme), "0"));
        set
        {
            SettingHelper.Write(nameof(ApplicationTheme), value.ToString());
            ((FrameworkElement)MainWindow.Current.Content).RequestedTheme = (ElementTheme)value;
            OnPropertyChanged();
        }
    }
}
