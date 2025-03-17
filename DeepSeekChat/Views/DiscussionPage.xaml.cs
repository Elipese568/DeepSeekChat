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
    }
}
