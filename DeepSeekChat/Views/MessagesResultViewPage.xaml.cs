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
    public sealed partial class MessagesResultViewPage : Page
    {
        private MessagesResultViewModel ViewModel { get; set; }
        public MessagesResultViewPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var typedParam = e.Parameter as MessagesResultGroup;
            ViewModel = new()
            {
                DiscussionItemViewModel = typedParam.DiscussionItem,
                MessagesViewModel = typedParam.MessagesViewModel,
                QueryString = typedParam.QueryString
            };
            base.OnNavigatedTo(e);
        }

        private void MessagesView_GotoMessage(object sender, RoutedEventArgs e)
        {
            var msgVm = (sender as Button).DataContext as ApplicationChatMessageViewModel;
            MainPage.Current.NavigateToDiscussion(new DiscussionViewNavigationParameters()
            {
                ItemViewModel = ViewModel.DiscussionItemViewModel,
                LocateMessageObject = msgVm,
                Mode = DiscussionNavigationMode.LocateMessage
            },null,true);
        }
    }
}
