using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeepSeekChat.Helper;
using DeepSeekChat.ViewModels;
using DeepSeekChat.Views;
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
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DeepSeekChat.Controls
{
    [ObservableObject]
    public sealed partial class ChatOptionSettingPane : UserControl
    {
        [RelayCommand]
        public async Task DetailEditSystemPrompt()
        {
            TextBox textBox = new()
            {
                MaxLength = int.MaxValue,
                AcceptsReturn = true,
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch,
                VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Stretch,
                TextWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap
            };
            ScrollViewer.SetVerticalScrollBarVisibility(textBox, ScrollBarVisibility.Visible);
            textBox.Text = ChatOptionsViewModel.SystemPrompt;
            var contentDialog = ContentDialogHelper.CreateContentDialog(
                "EditSystemPromptDialogHeader".GetLocalized("DiscussionPage"),
                textBox,
                "ConfirmText".GetLocalized(),
                "CancelText".GetLocalized(),
                null,
                ContentDialogButton.Primary,
                MainPage.Current.Content.XamlRoot);
            if (await contentDialog.ShowAsync() == ContentDialogResult.Primary)
            {
                ChatOptionsViewModel.SystemPrompt = textBox.Text;
            }
        }

        [RelayCommand]
        public void RandomSeed()
        {
            ChatOptionsViewModel.Seed = Random.Shared.Next();
        }

        public ChatOptionsViewModel ChatOptionsViewModel
        {
            get { return (ChatOptionsViewModel)GetValue(ChatOptionsViewModelProperty); }
            set { SetValue(ChatOptionsViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChatOptions.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChatOptionsViewModelProperty =
            DependencyProperty.Register("ChatOptionsViewModel", typeof(ChatOptionsViewModel), typeof(ChatOptionSettingPane), new PropertyMetadata(null));

        public ChatOptionSettingPane()
        {
            InitializeComponent();
        }
    }
}
