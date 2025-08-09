using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using DeepSeekChat.ViewModels;
using DeepSeekChat.Models;
using System.Threading.Tasks;
using Windows.UI;
using System.Diagnostics;
using DeepSeekChat.Service;
using Windows.ApplicationModel.Resources.Core;
using Windows.ApplicationModel.Resources;
using DeepSeekChat.Helper;
using System.Threading;
using Microsoft.UI.Dispatching;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DeepSeekChat.Views
{
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
                    MainPage_obj1_Bindings__Connect(6, RemoveDiscussionMenuItem);
                    MainPage_obj1_Bindings__Connect(7, ChangeDiscussionTitleMenuItem);

                    UpdateLayout();
                    Bindings.Update();

                    UpdateDiscussionItemNavList();
                    
                    GC.Collect();
                });

            };
        }
        private readonly ResourceContext _defaultContextForCurrentView;
        private readonly SettingService _settingService;

        private void ListViewItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ViewModel.OperatingItem = ((sender as FrameworkElement).Tag as DiscussionItemViewModel);
            RightClickCommands.ShowAt(sender as UIElement, e.GetPosition(sender as UIElement));
        }

        private void DiscussList_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if(args.IsSettingsSelected)
            {
                ContentFrame.Navigate(typeof(SettingPage), _needSetServerEndpoint ? 1 : _needSetApiKey ? 0 : null, args.RecommendedNavigationTransitionInfo);
                _needSetServerEndpoint = false;
                _needSetApiKey = false;
            }
            else if(args.SelectedItem is NavigationViewItem navitem && navitem.DataContext is DiscussionItemViewModel divm)
            {
                ViewModel.SelectedDiscussItem = divm;
                ViewModel.SelectedDiscussItem.LeastStatus = ProgressStatus.None;

                ContentFrame.Navigate(typeof(DiscussionPage), divm, args.RecommendedNavigationTransitionInfo);
            }
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
    }
}
