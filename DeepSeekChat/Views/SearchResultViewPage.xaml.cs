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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Search;
using Windows.ApplicationModel.Search.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DeepSeekChat.Views;

public class SearchResultNavigationParameters
{
    public string QueryString { get; set; }
    public List<DiscussionsResultGroup> DiscussionResults { get; set; }
    public List<MessagesResultGroup> MessageResults { get; set; }
}

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SearchResultViewPage : Page
{
    private SearchResultViewModel ViewModel { get; set; }
    public SearchResultViewPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        var parameters = e.Parameter as SearchResultNavigationParameters;
        ViewModel = new SearchResultViewModel()
        {
            QueryString = parameters.QueryString,
            DiscussionResults = parameters.DiscussionResults,
            MessageResults = parameters.MessageResults
        };
        base.OnNavigatedTo(e);
    }

    private void MessagesView_GotoMessage(object sender, RoutedEventArgs e)
    {
        var msgVm = (sender as Button).DataContext as ApplicationChatMessageViewModel;
        MainPage.Current.NavigateToDiscussion(new DiscussionViewNavigationParameters()
        {
            ItemViewModel = ((MessagesResultGroup)MessageListSelector.SelectedItem).DiscussionItem,
            LocateMessageObject = msgVm,
            Mode = DiscussionNavigationMode.LocateMessage
        }, null, true);
    }
}
