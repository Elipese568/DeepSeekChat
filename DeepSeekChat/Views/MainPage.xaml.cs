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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModel { get; set; }

        public static MainPage Current { get; private set; }

        public MainPage()
        {
            ViewModel = new(this);
            DataContext = ViewModel;

            ViewModel.DiscussionItemViewModels.CollectionChanged += DiscussItems_CollectionChanged;
            ViewModel.DiscussionViewStatusChanged += ViewModel_DiscussionViewStatusChanged;
            Current = this;
            this.InitializeComponent();

            _settingService = App.Current.GetService<SettingService>();

            //UNSUPPORTED: Dynamic switch language
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
                    MainPage_obj1_Bindings__Connect(3, RemoveDiscussionMenuItem);
                    MainPage_obj1_Bindings__Connect(4, ChangeDiscussionTitleMenuItem);

                    UpdateLayout();
                    Bindings.Update();

                    
                    GC.Collect();
                });

            };
        }
        private readonly ResourceContext _defaultContextForCurrentView;
        private readonly SettingService _settingService;

        private void ViewModel_DiscussionViewStatusChanged(object? sender, DiscussionViewStatusChangedEventArgs e)
        {
            //if(_self_set_property_no_process_flag_dont_remove)
            //{
            //    _self_set_property_no_process_flag_dont_remove = false;
            //    return;
            //}
            //_self_set_property_no_process_flag_dont_remove = true;
            //var item = DiscussList.ItemsPanelRoot.Children.First(x => ((x as ListViewItem).DataContext as DiscussItemViewModel).Id == e.DiscussItem.Id) as ListViewItem;
            //((item.Content as Grid).FindName("StatusSelector") as Border).Visibility = Visibility.Visible;
        }

        private void DiscussItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                DiscussList.Visibility = Visibility.Visible;
                NoItemTip.Visibility = Visibility.Collapsed;
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove && ViewModel.DiscussionItemViewModels.Count == 0)
            {
                DiscussList.Visibility = Visibility.Collapsed;
                NoItemTip.Visibility = Visibility.Visible;
            }
        }

        private void ListViewItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ViewModel.OperatingItem = ((sender as FrameworkElement).Tag as DiscussionItemViewModel);
            ViewModel.OperatingItem.IsViewed = true;
            RightClickCommands.ShowAt(sender as UIElement, e.GetPosition(sender as UIElement));
        }

        private void DiscussList_ItemClick(object sender, ItemClickEventArgs e)
        {
          
        }

        private void DiscussList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.RemovedItems.Count > 0 && e.AddedItems.Count == 0)
            {
                ViewModel.SelectedDiscussItem = null;
            }
            else
            {
                ViewModel.SelectedDiscussItem = (e.AddedItems[0] as DiscussionItemViewModel);
                ViewModel.SelectedDiscussItem.LeastStatus = ProgressStatus.None;
                ViewModel.TryNavigate(ViewModel.SelectedDiscussItem.Id.ToString(), ()=> new DiscussionPage(ViewModel.SelectedDiscussItem));

            }
        }

        int _goSettingButtonContinuousClickCount = 0;
        CancellationTokenSource _goSettingButtonContinousCts = null;
        Stopwatch _advanceOperationPagenavigatedWatch = new();
        private void GoSettingButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentPageId == "AdvanceOperation" && _advanceOperationPagenavigatedWatch.ElapsedMilliseconds < 2000)
            {
                return;
            }
            _advanceOperationPagenavigatedWatch.Reset();
            _goSettingButtonContinuousClickCount++;
            ViewModel.TryNavigate("Setting", () => new SettingPage());
            DiscussList.SelectedIndex = -1;
            if(_goSettingButtonContinousCts == null)
            {
                _goSettingButtonContinousCts = new CancellationTokenSource();
                Task.Run(() =>
                {
                    while (!_goSettingButtonContinousCts.IsCancellationRequested)
                    {
                        if(_goSettingButtonContinuousClickCount == 10)
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                Current.ViewModel.TryNavigate("AdvanceOperation", () => new AdvanceOperationPage(), true);
                                _advanceOperationPagenavigatedWatch.Start();
                            });
                            _goSettingButtonContinuousClickCount = 0;
                            _goSettingButtonContinousCts.Cancel();
                        }
                    }
                    _goSettingButtonContinousCts = null;
                }, _goSettingButtonContinousCts.Token);
                _goSettingButtonContinousCts?.CancelAfter(2000);
            }
        }

        private void SetApiKeyButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.TryNavigate("SettingTipApiKey", () => new SettingPage(true), true);
            DiscussList.SelectedIndex = -1;
        }

        public bool ReverseBool(bool value)
        {
            return !value;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ((FrameworkElement)MainWindow.Current.Content).RequestedTheme = (ElementTheme)int.Parse(App.Current.GetService<SettingService>().Read("ApplicationTheme", "0"));
        }
    }
}
