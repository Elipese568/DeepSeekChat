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

            ViewModel.DiscussItemViewModels.CollectionChanged += DiscussItems_CollectionChanged;
            ViewModel.DiscussionViewStatusChanged += ViewModel_DiscussionViewStatusChanged;
            Current = this;

            this.InitializeComponent();
        }

        private bool _self_set_property_no_process_flag_dont_remove = false;
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
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove && ViewModel.DiscussItemViewModels.Count == 0)
            {
                DiscussList.Visibility = Visibility.Collapsed;
                NoItemTip.Visibility = Visibility.Visible;
            }
        }

        private void ListViewItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ViewModel.OperatingItem = ((sender as FrameworkElement).Tag as DiscussItemViewModel);
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
                ViewModel.SelectedDiscussItem = (e.AddedItems[0] as DiscussItemViewModel);
                ViewModel.SelectedDiscussItem.LeastStatus = ProgressStatus.None;
                ViewModel.TryNavigate(ViewModel.SelectedDiscussItem.Id.ToString(), ()=> new DiscussionPage(ViewModel.SelectedDiscussItem));

            }
        }
        private void GoSettingButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.TryNavigate("Setting", () => new SettingPage());
            DiscussList.SelectedIndex = -1;
        }

        public static Visibility Transparent2Visibility(Color color)
        {
            return color.A > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ((FrameworkElement)MainWindow.Current.Content).RequestedTheme = (ElementTheme)int.Parse(Helper.SettingHelper.Read("ApplicationTheme", "0"));
        }
    }
}
