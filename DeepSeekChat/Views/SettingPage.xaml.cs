using DeepSeekChat.Helper;
using DeepSeekChat.Models;
using DeepSeekChat.Service;
using DeepSeekChat.ViewModels;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources.Core;
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
        private readonly ResourceContext _defaultContextForCurrentView;
        public SettingPage()
        {
            ViewModel = new(this);
            this.InitializeComponent();

            _defaultContextForCurrentView = ResourceManager.Current.DefaultContext;
            _defaultContextForCurrentView.QualifierValues.MapChanged += async (s, m) =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    //UNSUPPORTED: Dynamic switch language
                    _contentLoaded = false;
                    InitializeComponent();
                    UpdateLayout();
                    Bindings.Update();
                    GC.Collect();
                });

                DispatcherQueue.TryEnqueue(() =>
                {
                    Grid grid = new();
                    grid.Background = (SolidColorBrush)Application.Current.Resources["AccentFillColorDefaultBrush"];

                    int w = (int)MainWindow.Current.Bounds.Width, h = (int)MainWindow.Current.Bounds.Height;
                    grid.Height = MainWindow.Current.Bounds.Height;
                    grid.Width = MainWindow.Current.Bounds.Width;

                    grid.Children.Add(new TextBlock()
                    {
                        Text = "ChangingLanguageDisplayText".GetLocalized(),
                        Foreground = (SolidColorBrush)Application.Current.Resources["TextOnAccentFillColorPrimaryBrush"],
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Style = (Style)Application.Current.Resources["SubtitleTextBlockStyle"]
                    });

                    Storyboard sb = new();
                    DoubleAnimationUsingKeyFrames kfs = new();
                    kfs.KeyFrames.Add(new DiscreteDoubleKeyFrame()
                    {
                        Value = 0,
                        KeyTime = TimeSpan.FromSeconds(0),
                    });
                    kfs.KeyFrames.Add(new EasingDoubleKeyFrame()
                    {
                        Value = 1,
                        KeyTime = TimeSpan.FromSeconds(0.35),
                        EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut }
                    });
                    kfs.KeyFrames.Add(new DiscreteDoubleKeyFrame()
                    {
                        Value = 1,
                        KeyTime = TimeSpan.FromSeconds(1)
                    });
                    kfs.KeyFrames.Add(new EasingDoubleKeyFrame()
                    {
                        Value = 0,
                        KeyTime = TimeSpan.FromSeconds(1.35),
                        EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseIn }
                    });

                    Storyboard.SetTargetProperty(kfs, "Opacity");
                    Storyboard.SetTarget(kfs, grid);

                    sb.Children.Add(kfs);


                    ((Grid)MainWindow.Current.Content).Children.Add(grid);

                    sb.Begin();

                    sb.Completed += (s, e) =>
                    {
                        ((Grid)MainWindow.Current.Content).Children.Remove(grid);
                        grid = null;
                        GC.Collect();
                    };
                });
            };
        }

        public SettingPage(bool tipApiKeyOption) : this()
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
            //if((e.AddedItems[0] as ComboBoxItem).Content.Equals("Light"))
            //    ThemeTeachingTip.IsOpen = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(ModelNameTextBox.Text) || string.IsNullOrWhiteSpace(ModelIDTextBox.Text))
            {
                ArgWrongTip.Visibility = Visibility.Visible;
                AddButton.IsEnabled = false;
                return;
            }
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
                await ContentDialogHelper.ShowNoActionMessageDialog("UnexpectedOperationText".GetLocalized(), "InvaildRemoveDefaultModelMessage".GetLocalized("SettingPage"), XamlRoot);
                return;
            }

            ViewModel.RemoveModel(ViewModel.SelectedModel);
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var modelManager = App.Current.GetService<ModelsManagerService>();
            if (ViewModel.SelectedModel.ToString().ToUpper() is AiModelStorage.DEEPSEEK_DEFAULT_MODEL_GUID or AiModelStorage.DEEPSEEK_PRO_MODEL_GUID)
            {
                await ContentDialogHelper.ShowNoActionMessageDialog("UnexpectedOperationText".GetLocalized(), "InvaildModifyDefaultModelInformationMessage".GetLocalized("SettingPage"), XamlRoot);
                return;
            }

            AiModel model = modelManager.GetModelById(ViewModel.SelectedModel);

            var modifyContentDialog = ContentDialogHelper.CreateContentDialog(
                "ModifyModelOptionsDialogTitle".GetLocalized("SettingPage"),
                null,
                "ConfirmText".GetLocalized(),
                "CancelText".GetLocalized(),
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
                Header = "ModelNameTextBox.Header".GetLocalized("SettingPage"),
                Text = model.Name,
                PlaceholderText = "ModelNameTextBox.PlaceholderText".GetLocalized("SettingPage"),
                Name = "ModelNameChangeTextBox",
            };

            var ErrorBar = new InfoBar()
            {
                IsOpen = false,
                Visibility = Visibility.Collapsed,
                IsClosable = false,
                Severity = InfoBarSeverity.Error,
                Title = "Error",
                Message = "ArgWrongTip.Text".GetLocalized("SettingPage")
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
                Header = "ModelDescriptionTextBox.Header".GetLocalized("SettingPage"),
                Text = model.Description,
                PlaceholderText = "ModelDescriptionTextBox.PlaceholderText".GetLocalized("SettingPage"),
                Name = "ModelDescriptionChangeTextBox"
            };

            content.Children.Add(ModelNameChangeTextBox);
            content.Children.Add(ErrorBar);
            content.Children.Add(ModelDescriptionChangeTextBox);
            content.Children.Add(new TextBox()
            {
                Header = "ModelIdTextBox.Header".GetLocalized("SettingPage"),
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
