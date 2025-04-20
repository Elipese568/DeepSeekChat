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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(ModelNameTextBox.Text) || string.IsNullOrEmpty(ModelIDTextBox.Text))
            {
                ArgWrongTip.IsOpen = true;
                return;
            }
            ViewModel.AddModel(ModelNameTextBox.Text, ModelDescriptionTextBox.Text, ModelIDTextBox.Text);
            ClearAddModelInfo();
        }

        private void ClearAddModelInfo()
        {
            ModelNameTextBox.Text = string.Empty;
            ModelDescriptionTextBox.Text = string.Empty;
            ModelIDTextBox.Text = string.Empty;
        }

        private void Flyout_Closed(object sender, object e)
        {
            ClearAddModelInfo();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ClearAddModelInfo();
            AddModelFlyout.Hide();
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedModel.ToString().ToUpper() is "545B7456-BCF5-4E19-9E23-6C08AD3A90A3" or "F72AB0EC-37D3-43F8-BCC7-A04BBD9B2A37")
            {
                ContentDialog contentDialog = new();
                contentDialog.XamlRoot = this.XamlRoot;
                contentDialog.Title = "Unexpected Operation";
                contentDialog.Content = "Cannot remove default model.";
                contentDialog.CloseButtonText = "OK";
                contentDialog.CloseButtonStyle = (Style)App.Current.Resources["AccentButtonStyle"];
                contentDialog.RequestedTheme = RequestedTheme;
                await contentDialog.ShowAsync();
                return;
            }

            ViewModel.RemoveModel(ViewModel.SelectedModel);
        }
    }
}
