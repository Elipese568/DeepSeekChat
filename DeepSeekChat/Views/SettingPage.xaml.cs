using DeepSeekChat.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DeepSeekChat.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        public SettingViewModel ViewModel { get; set; }

        public SettingPage()
        {
            ViewModel = new();
            this.InitializeComponent();
        }

        private void SettingsCard_Click(object sender, RoutedEventArgs e)
        {
            Launcher.LaunchUriAsync(new("https://github.com/Elipese568/DeepSeekChat/issues"));
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if((e.AddedItems[0] as ComboBoxItem).Content.Equals("Light"))
                ThemeTeachingTip.IsOpen = true;
        }
    }
}
