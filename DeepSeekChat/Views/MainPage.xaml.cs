using DeepSeekChat.Helper;
using DeepSeekChat.Models;
using DeepSeekChat.Service;
using DeepSeekChat.ViewModels;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Resources.Core;
using Windows.ApplicationModel.Search;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DeepSeekChat.Views;


public class DiscussionItemNavigationItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate DiscussionItemItemTemplate { get; set; }
    public DataTemplate NavigationViewItemTemplate { get; set; }

    public DiscussionItemNavigationItemTemplateSelector()
    {
    }

    protected override DataTemplate SelectTemplateCore(object item)
    {
        if (item is NavigationViewItem)
            return NavigationViewItemTemplate;

        return DiscussionItemItemTemplate;
    }
}

public class MessagesResultGroup
{
    public MessagesResultGroup(IEnumerable<ApplicationChatMessageViewModel> messages, DiscussionItemViewModel item, string queryString)
    {
        MessagesViewModel = new([..messages]);
        DiscussionTitle = item.Title;
        DiscussionItem = item;
        QueryString = queryString;
    }
    public string DiscussionTitle { get; set; }
    public DiscussionItemViewModel DiscussionItem { get; set; }
    public string QueryString { get; set; }
    public MessagesViewModel MessagesViewModel { get; set; }

    public override string ToString()
    {
        return DiscussionTitle;
    }
}

public class DiscussionsResultGroup
{
    public DiscussionsResultGroup(DiscussionItemViewModel item)
    {
        Title = item.Title;
        Item = item;
    }

    public string Title { get; set; }
    public DiscussionItemViewModel Item { get; set; }

    public override string ToString()
    {
        return Title;
    }
}
public class SearchResultGroup : IGrouping<string, object>
{
    private List<object> _results;
    public SearchResultGroup(IEnumerable<object> items, string header)
    {
        _results = new(items ?? []);
        Key = header;
    }

    public string Key { get; set; }

    public IEnumerator<object> GetEnumerator()
    {
        return _results.GetEnumerator();
    }

    public override string ToString()
    {
        return Key;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class NoItemSearchGroup : SearchResultGroup
{
    public NoItemSearchGroup() : base(null, "")
    {
    }
}

public class SearchTypeTemplateSelector : DataTemplateSelector
{
    public DataTemplate DiscussionItemSearchTemplate { get; set; }
    public DataTemplate MessageSearchTemplate { get; set; }
    public DataTemplate NoItemSearchTemplate { get; set; }
    protected override DataTemplate SelectTemplateCore(object item)
    {
        if (item is MessagesResultGroup)
            return MessageSearchTemplate;
        else if (item is DiscussionsResultGroup)
            return DiscussionItemSearchTemplate;
        else
            return NoItemSearchTemplate;
    }
}

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainPageViewModel ViewModel { get; set; }
    public new Frame Frame => ContentFrame;

    public static MainPage Current { get; private set; }

    public MainPage()
    {
        ViewModel = new(this);
        DataContext = ViewModel;

        Current = this;
        this.InitializeComponent();

        _settingService = App.Current.GetService<SettingService>();
        _discussionItemService = App.Current.GetService<DiscussionItemService>();
        _defaultContextForCurrentView = ResourceManager.Current.DefaultContext;

        _defaultContextForCurrentView.QualifierValues.MapChanged += async (s, m) =>
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                RemoveDiscussionMenuItem.Text = "RemoveMenuItem.Text".GetLocalized("MainPage");
                ChangeDiscussionTitleMenuItem.Text = "ChangeTitleMenuItem.Text".GetLocalized("MainPage");
                RemoveDiscussionMenuItem.Language = _settingService.Read(SettingService.SETTING_DISPLAY_LANGUAGE, "zh-Hans-CN");
                ChangeDiscussionTitleMenuItem.Language = _settingService.Read(SettingService.SETTING_DISPLAY_LANGUAGE, "zh-Hans-CN");
                _contentLoaded = false;
                InitializeComponent();

                var MainPage_obj1_Bindings__Connect = DynamicCall.GetVoidInvoker<IMainPage_Bindings, int, object>(Bindings, "Connect");
                MainPage_obj1_Bindings__Connect(7, RemoveDiscussionMenuItem);
                MainPage_obj1_Bindings__Connect(8, ChangeDiscussionTitleMenuItem);

                UpdateLayout();
                Bindings.Update();

                UpdateDiscussionItemNavList();
                
                GC.Collect();
            });

        };
    }
    private readonly ResourceContext _defaultContextForCurrentView;
    private readonly SettingService _settingService;
    private readonly DiscussionItemService _discussionItemService;

    private void ListViewItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        ViewModel.OperatingItem = ((sender as FrameworkElement).Tag as DiscussionItemViewModel);
        RightClickCommands.ShowAt(sender as UIElement, e.GetPosition(sender as UIElement));
    }
    private bool _internal_process = false;
    private void DiscussList_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if(_internal_process)
        {
            _internal_process = false; // _internal_process can influence action of "SelectionChanged", so we set it false after setting selection
            return;
        }
        if (args.IsSettingsSelected)
        {
            ContentFrame.Navigate(typeof(SettingPage), _needSetServerEndpoint ? 1 : _needSetApiKey ? 0 : null, args.RecommendedNavigationTransitionInfo);
            _needSetServerEndpoint = false;
            _needSetApiKey = false;
            return;
        }
        DiscussionItemViewModel discussionItem = ((args.SelectedItem as NavigationViewItem)?.DataContext as DiscussionItemViewModel) ?? args.SelectedItem as DiscussionItemViewModel;
        if (discussionItem == null)
            return;
        NavigateToDiscussion(new()
        {
            ItemViewModel = discussionItem,
            Mode = DiscussionNavigationMode.Navigate
        }, args.RecommendedNavigationTransitionInfo);
    }

    public void NavigateToDiscussion(DiscussionViewNavigationParameters parameter, NavigationTransitionInfo? nti, bool setListSelected = false)
    {
        if (setListSelected)
        {
            _internal_process = true;
            DiscussList.SelectedItem = parameter.ItemViewModel;
            
        }
        ViewModel.SelectedDiscussItem = parameter.ItemViewModel;
        ViewModel.SelectedDiscussItem.LeastStatus = ProgressStatus.None;

        ContentFrame.Navigate(typeof(DiscussionPage), parameter, nti??new DrillInNavigationTransitionInfo());
    }

    private bool _needSetApiKey = false;
    private bool _needSetServerEndpoint = false;
    private void SetApiKeyButton_Click(object sender, RoutedEventArgs e)
    {
        _needSetApiKey = true;
        DiscussList.SelectedItem = DiscussList.SettingsItem;
    }

    private void SetServerEndpointButton_Click(object sender, RoutedEventArgs e)
    {
        _needSetServerEndpoint = true;
        DiscussList.SelectedItem = DiscussList.SettingsItem;
    }

    public bool ReverseBool(bool value)
    {
        return !value;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        ((FrameworkElement)MainWindow.Current.Content).RequestedTheme = (ElementTheme)int.Parse(App.Current.GetService<SettingService>().Read("ApplicationTheme", "0"));
        UpdateDiscussionItemNavList();

        ViewModel.DiscussionItemViewModels.CollectionChanged += (s, e) =>
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                NavigationViewItem navitem = new()
                {
                    DataContext = (DiscussionItemViewModel)e.NewItems[0],
                    ContentTemplate = "DiscussionItemNavigationViewItemDataTemplate".GetResource<DataTemplate>()
                };

                DiscussList.MenuItems.Add(navitem);
            }
            else
            {
                int removeItemIdx = DiscussList.MenuItems.IndexOf(x => (x as NavigationViewItem).DataContext is DiscussionItemViewModel divm && divm.Id == (e.OldItems[0] as DiscussionItemViewModel).Id);
                DiscussList.MenuItems.RemoveAt(removeItemIdx);
            }
        };
    }

    private void UpdateDiscussionItemNavList()
    {
        foreach (
            var navitem in
            from item in ViewModel.DiscussionItemViewModels
            let navitem = new NavigationViewItem()
            {
                DataContext = item,
                ContentTemplate = "DiscussionItemNavigationViewItemDataTemplate".GetResource<DataTemplate>()
            }
            select navitem
        )
        {
            DiscussList.MenuItems.Add(navitem);
        }
    }

    private async void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        try
        {
            if(args.InvokedItemContainer.Tag is string tagStr)
            {
                switch (tagStr)
                {
                    case "AddDiscussionItem":
                        await ViewModel.AddDiscussion();
                        break;
                    case "QuickDiscussion":
                        await ViewModel.QuickDiscussion();
                        break;
                }
            }
        }
        catch { }
    }

    private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
            return;

        var queryText = sender.Text;
        if(queryText == string.Empty) return;
        var nameQueryResult = 
            from discussionItem in ViewModel.DiscussionItemViewModels
            where discussionItem.Title.Contains(queryText) 
            select new DiscussionsResultGroup(discussionItem);
        var messagesQueryResult =
            from discussionItem in ViewModel.DiscussionItemViewModels
            where discussionItem.MessagesViewModel.MessageViewModels.Any(
                x => x.UserPrompt.Contains(queryText) ||
                x.AiChatCompletion.Content.Contains(queryText) ||
                x.AiChatCompletion.ReasoningContent.Contains(queryText))
            select discussionItem into matched
            from msg in matched.MessagesViewModel.MessageViewModels
            where msg.UserPrompt.Contains(queryText) ||
                  msg.AiChatCompletion.Content.Contains(queryText) ||
                  msg.AiChatCompletion.ReasoningContent.Contains(queryText)
            group msg by matched into matchedMsgsGroup
            select new MessagesResultGroup(matchedMsgsGroup, matchedMsgsGroup.Key, queryText);

        ObservableCollection<object> result = [
            ..nameQueryResult,
            ..messagesQueryResult
        ];

        if (result.Count == 0)
            result.Add(new NoItemSearchGroup());
        sender.ItemsSource = result;
    }

    private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        
    }

    private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if(args.ChosenSuggestion == null)
        {
            var results = (sender.ItemsSource as ObservableCollection<object>);
            Frame.Navigate(typeof(SearchResultViewPage), new SearchResultNavigationParameters()
            {
                QueryString = args.QueryText,
                DiscussionResults = results.Where(x => x is DiscussionsResultGroup).Cast<DiscussionsResultGroup>().ToList(),
                MessageResults = results.Where(x => x is MessagesResultGroup).Cast<MessagesResultGroup>().ToList()
            });
        }

        if (args.ChosenSuggestion is NoItemSearchGroup)
            return;

        if (args.ChosenSuggestion is DiscussionsResultGroup divmResult)
        {
            DiscussList.SelectedItem = divmResult.Item;
        }

        if (args.ChosenSuggestion is MessagesResultGroup msgsResult)
        {
            Frame.Navigate(typeof(MessagesResultViewPage), msgsResult);
        }
        DiscussList.SelectedItem = null;
    }

    public void NavigateToSetting()
    {
        DiscussList.SelectedItem = DiscussList.SettingsItem;
    }
}
