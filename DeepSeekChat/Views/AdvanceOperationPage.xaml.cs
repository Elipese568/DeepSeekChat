using DeepSeekChat.Service;
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
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DeepSeekChat.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AdvanceOperationPage : Page
    {
        public AdvanceOperationPage()
        {
            this.InitializeComponent();
        }
        private void OpenChatMessagesStorageFile_Click(object sender, RoutedEventArgs e)
        {
            Launcher.LaunchUriAsync(new(App.Current.GetService<DiscussionItemService>().GetStorageFilePath()));
        }

        private async void CreateChatMessageBackupResetMessages_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new();
            var window = MainWindow.Current;

            WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(window));

            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.ViewMode = PickerViewMode.List;
            picker.FileTypeFilter.Add("*");

            var jsonFile = await picker.PickSingleFileAsync();
            if (jsonFile == null)
                return;

            var discussionItemService = App.Current.GetService<DiscussionItemService>();
            var originfile = await StorageFile.GetFileFromPathAsync(discussionItemService.GetStorageFilePath());
            await originfile.CopyAndReplaceAsync(jsonFile);
            discussionItemService
                .GetStroragedDiscussionItems()
                .Select(x => x.Id)
                .ToList()
                .ForEach(discussionItemService.RemoveDiscussionItem);

            var dialog = new ContentDialog
            {
                Title = "Backup and Reset",
                Content = "Chat messages have been backed up and reset successfully.",
                CloseButtonText = "OK",
                XamlRoot = window.Content.XamlRoot
            };
            await dialog.ShowAsync();
        }
    }
}
