using DeepSeekChat.Helper;
using DeepSeekChat.Models;
using DeepSeekChat.Service;
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
            ViewModel = new(this);
            this.InitializeComponent();
        }

        public SettingPage(bool tipApiKeyOption)
        {
            ViewModel = new(this);
            this.InitializeComponent();
            if (tipApiKeyOption)
            {
                ApiKeyTip.IsOpen = true;
            }
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
            ViewModel.AddModel(ModelNameTextBox.Text, ModelDescriptionTextBox.Text, ModelIDTextBox.Text);
            ClearAddModelInfo();
            AddModelFlyout.Hide();
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
            if (ViewModel.SelectedModel.ToString().ToUpper() is AiModelStorage.DEEPSEEK_DEFAULT_MODEL_GUID or AiModelStorage.DEEPSEEK_PRO_MODEL_GUID)
            {
                await ContentDialogHelper.ShowNoActionMessageDialog("Unexpected Operation", "Cannot remove default model.", XamlRoot);
                return;
            }

            ViewModel.RemoveModel(ViewModel.SelectedModel);
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var modelManager = App.Current.GetService<ModelsManagerService>();
            if (ViewModel.SelectedModel.ToString().ToUpper() is AiModelStorage.DEEPSEEK_DEFAULT_MODEL_GUID or AiModelStorage.DEEPSEEK_PRO_MODEL_GUID)
            {
                await ContentDialogHelper.ShowNoActionMessageDialog("Unexpected Operation", "Cannot modify infomation of default model.", XamlRoot);
                return;
            }

            AiModel model = modelManager.GetModelById(ViewModel.SelectedModel);

            var modifyContentDialog = ContentDialogHelper.CreateContentDialog(
                "Modify Options",
                null,
                "Confirm",
                "Cancel",
                null,
                ContentDialogButton.Primary,
                this.XamlRoot);

            StackPanel content = new()
            {
                Spacing = 16,
                Orientation = Orientation.Vertical,
                MinWidth = 300
            };

            var ModelNameChangeTextBox = new TextBox()
            {
                Header = "Model Name",
                Text = model.Name,
                PlaceholderText = "Enter a name of the model",
                Name = "ModelNameChangeTextBox",
            };

            var ErrorBar = new InfoBar()
            {
                IsOpen = false,
                Visibility = Visibility.Collapsed,
                IsClosable = false,
                Severity = InfoBarSeverity.Error,
                Title = "Error",
                Message = "Name of model cannot be empty or white space"
            };

            ModelNameChangeTextBox.TextChanged += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(ModelNameChangeTextBox.Text))
                {
                    modifyContentDialog.IsPrimaryButtonEnabled = false;
                    ErrorBar.Visibility = Visibility.Visible;
                    ErrorBar.IsOpen = true;
                }
                else if(!string.IsNullOrWhiteSpace(ModelNameChangeTextBox.Text) && ErrorBar.IsOpen == true)
                {
                    ErrorBar.IsOpen = false;
                    ErrorBar.Visibility = Visibility.Collapsed;
                    modifyContentDialog.IsPrimaryButtonEnabled = true;
                }
            };
            var ModelDescriptionChangeTextBox = new TextBox()
            {
                Header = "Model Description",
                Text = model.Description,
                PlaceholderText = "Enter a description of the model",
                Name = "ModelDescriptionChangeTextBox"
            };

            content.Children.Add(ModelNameChangeTextBox);
            content.Children.Add(ErrorBar);
            content.Children.Add(ModelDescriptionChangeTextBox);
            content.Children.Add(new TextBox()
            {
                Header = "Model ID",
                Text = model.ModelID,
                IsEnabled = false
            });

            modifyContentDialog.Content = content;

            var result = await ContentDialogHelper.ShowContentDialog(modifyContentDialog);

            if (result != ContentDialogResult.Primary)
            {
                return;
            }

            var vm = ViewModel.GetModelViewModel(ViewModel.SelectedModel);

            vm.Name = ModelNameChangeTextBox.Text;
            vm.Description = ModelDescriptionChangeTextBox.Text;
            return;
        }

        private void ModelNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ModelNameTextBox.Text) || string.IsNullOrWhiteSpace(ModelIDTextBox.Text))
            {
                ArgWrongTip.Visibility = Visibility.Visible;
                AddButton.IsEnabled = false;
            }
            else if(ArgWrongTip.Visibility == Visibility.Visible)
            {
                ArgWrongTip.Visibility = Visibility.Collapsed;
                AddButton.IsEnabled = true;
            }
        }

        private void Image_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            (sender as Image).Opacity = 1;
        }

        private void Image_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            (sender as Image).Opacity = 0;
        }
    }
}
