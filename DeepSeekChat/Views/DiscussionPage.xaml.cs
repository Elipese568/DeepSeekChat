using CommunityToolkit.WinUI.Controls;
using CommunityToolkit.WinUI.UI.Controls;
using DeepSeekChat.Helper;
using DeepSeekChat.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DeepSeekChat.Views
{

    public class StreamingModeToSelectionModeConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !(bool)value ? ListViewSelectionMode.Multiple : ListViewSelectionMode.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ModIdToDescriptiveConverter : IValueConverter
    {
        public const string DetailedRequest = "detailed_request";

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value switch
            {
                DetailedRequest => "DetailedRequestArgumentOption.Text".GetLocalized("DiscussionPage"),
                _ => value
            };
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class TrimStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((value as string) ?? "").Trim();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public sealed partial class DiscussionPage : Page
    {
        private class _ReplyMessageSelectSuggestionItem
        {
            private int _pri;
            public _ReplyMessageSelectSuggestionItem()
            {
                _pri = Random.Shared.Next();
            }

            public override int GetHashCode()
            {
                return _pri;
            }
        }
        public DiscussionViewModel ViewModel { get; set; }

        public DiscussionPage()
        {
            DataContext = ViewModel;
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel = new((DiscussionItemViewModel)e.Parameter);
            base.OnNavigatedTo(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OptionPane.IsPaneOpen = !OptionPane.IsPaneOpen;
        }

        private void StopGeneratingButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.StopGenerating();
        }

        private void TokenView_FileItemRemoving(object sender, CommunityToolkit.Labs.WinUI.TokenItemRemovingEventArgs e)
        {
            FileViewModel fileVm = e.Item as FileViewModel;
            ViewModel.RemoveFile(fileVm);
        }

        private void FilePresent_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            FileViewModel fileVm = ((Grid)sender).Tag as FileViewModel;
            ViewModel.PreviewFileViewModel = fileVm;
            ViewModel.FilePreviewerVisibility = Visibility.Visible;
        }

        private async void RemoveMessageButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await ContentDialogHelper.ShowMessageDialog("NeedConfirmOperationText".GetLocalized(), "DeleteMessageConfirmDialogText".GetLocalized("DiscussionPage"), "DeleteText".GetLocalized(), "CancelText".GetLocalized(), "", ContentDialogButton.Close, XamlRoot);

            if (result != ContentDialogResult.Primary)
                return;

            ApplicationChatMessageViewModel messageVm = (sender as Button).DataContext as ApplicationChatMessageViewModel;
            ViewModel.SelectedDiscussItemViewModel.MessagesViewModel.Remove(messageVm.InnerObject);
            if(ViewModel.ReferMessageViewModel?.Id == messageVm.Id)
            {
                ViewModel.ReferMessageViewModel = null;
                ViewModel.ReferVisibility = Visibility.Collapsed;
            }
        }

        private void RegenerateMessageButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SendKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            sender.IsEnabled = false; // 禁用加速器，防止重复触发

            if(ViewModel.SendCommand.CanExecute(ViewModel.InputingPrompt))
                ViewModel.SendCommand.Execute(ViewModel.InputingPrompt);

            args.Handled = true; // 标记事件已处理
        }

        private void FilePreviewPresenter_FilePreviewClosing(object sender, EventArgs e)
        {
            ViewModel.FilePreviewerVisibility = Visibility.Collapsed;
        }

        private void MessageInputBox_SuggestionRequested(
            CommunityToolkit.WinUI.Controls.RichSuggestBox sender, 
            CommunityToolkit.WinUI.Controls.SuggestionRequestedEventArgs args)
        {
            sender.ItemsSource = ViewModel.SelectedDiscussItemViewModel.FilesViewModel.FileViewModels.Where(x => x.Name.Contains(args.QueryText));
        }

        private void MessageInputBox_SuggestionChosen(
            CommunityToolkit.WinUI.Controls.RichSuggestBox sender, 
            CommunityToolkit.WinUI.Controls.SuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is FileViewModel fileVM)
                args.DisplayText = fileVM.Name;
            else if (args.SelectedItem is _ReplyMessageSelectSuggestionItem rmssi)
                args.DisplayText = "ReplyMessageWillDisplayHere";
        }

        private void MessageInputBox_TextChanged(CommunityToolkit.WinUI.Controls.RichSuggestBox sender, RoutedEventArgs args)
        {
            sender.TextDocument.GetText(Microsoft.UI.Text.TextGetOptions.None, out string text);
            text = text.TrimEnd('\r');
            ViewModel.InputingPrompt = text;
        }


        private async void MessagesView_ReferMessageEvent(object sender, RoutedEventArgs e)
        {
            ApplicationChatMessageViewModel messageVm = (sender as Button).DataContext as ApplicationChatMessageViewModel;
            ViewModel.ReferMessageViewModel = messageVm;
            ViewModel.ReferVisibility = Visibility.Visible;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ViewModel.ReferVisibility = Visibility.Collapsed;
            ViewModel.ReferMessageViewModel = null;
        }
    }
}
