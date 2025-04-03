using DeepSeekChat.Models;
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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DeepSeekChat.Views
{
    public class ProgressStatusVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (ProgressStatus)value == ProgressStatus.InProgress ? Visibility.Visible : Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DiscussionPage : Page
    {
        public DiscussionViewModel ViewModel { get; set; }

        public DiscussionPage(DiscussItem item)
        {
            ViewModel = new DiscussionViewModel(item); // 传递item到ViewModel
            this.InitializeComponent();

            // 确保消息更新时自动滚动到底部
            ViewModel.ScrollToBottomRequested += (s, e) =>
            {
                InvertedListView.ScrollIntoView(ViewModel.SelectedDiscussItem.Messages.LastOrDefault());
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OptionPane.IsPaneOpen = !OptionPane.IsPaneOpen;
        }

        private void StopGeneratingButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.StopGenerating();
        }
    }
}
