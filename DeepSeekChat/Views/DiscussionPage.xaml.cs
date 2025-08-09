using CommunityToolkit.WinUI.UI.Controls;
using DeepSeekChat.Helper;
using DeepSeekChat.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.IO;
using System.Linq;

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

    public sealed partial class DiscussionPage : Page
    {
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
    }
}
