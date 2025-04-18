using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeepSeekChat.Helper;
using DeepSeekChat.Models;
using DeepSeekChat.Views;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Windows.UI;
using System.ComponentModel;

namespace DeepSeekChat.ViewModels;

public class DiscussionViewStatusChangedEventArgs : EventArgs
{
    public ProgressStatus Status { get; set; }
    public DiscussItem DiscussItem { get; set; }
}

public partial class MainPageViewModel : ObservableRecipient
{
    private List<DiscussItem> _discussItems = new();

    public ObservableCollection<DiscussItemViewModel> DiscussItemViewModels { get; set; } = new();

    [ObservableProperty]
    private DiscussItemViewModel _operatingItem;

    [ObservableProperty]
    private DiscussItemViewModel _selectedDiscussItem;

    [ObservableProperty]
    private Page _contentPage;

    public event EventHandler<DiscussionViewStatusChangedEventArgs> DiscussionViewStatusChanged;

    public MainPage Parent { get; set; }

    public MainPageViewModel(MainPage page)
    {
        Parent = page;
    }

    [RelayCommand]
    public void RemoveDiscussion()
    {
        _discussItems.RemoveAt(_discussItems.IndexOf(x => x.Id == OperatingItem.Id));
        DiscussItemViewModels.Remove(DiscussItemViewModels.FirstOrDefault(x => x.Id == OperatingItem.Id));
        TryRemovePage(OperatingItem.Id.ToString());
    }

    [RelayCommand]
    public async Task ChangeDiscussionTitle()
    {
        int operatingIndex = _discussItems.IndexOf(x => x.Id == OperatingItem.Id);
        ContentDialog contentDialog = new();
        contentDialog.RequestedTheme = (MainWindow.Current.Content as FrameworkElement).RequestedTheme;
        contentDialog.Title = "Change Title";
        contentDialog.PrimaryButtonText = "Confirm";
        contentDialog.SecondaryButtonText = "Cancel";
        TextBox textBox = new()
        {
            Header = "Title",
            PlaceholderText = "Enter the new title of the discussion",
            MaxLength = 32
        };
        contentDialog.Content = textBox;
        contentDialog.PrimaryButtonClick += async (s, e) =>
        {
            if (textBox.Text.Length == 0)
            {
                e.Cancel = true;
            }
            else
            {
                DiscussItemViewModels[operatingIndex].Title = textBox.Text;
            }
        };
        contentDialog.DefaultButton = ContentDialogButton.Primary;
        contentDialog.XamlRoot = Parent.Content.XamlRoot;
        await contentDialog.ShowAsync();
    }

    [RelayCommand]
    public async Task AddDiscussion()
    {
        ContentDialog contentDialog = new();
        contentDialog.RequestedTheme = (MainWindow.Current.Content as FrameworkElement).RequestedTheme;
        contentDialog.Title = "Add Discussion";
        contentDialog.PrimaryButtonText = "Add";
        contentDialog.SecondaryButtonText = "Cancel";
        TextBox textBox = new()
        {
            Header = "Title",
            PlaceholderText = "Enter the title of the discussion",
            MaxLength = 32
        };
        contentDialog.Content = textBox;
        contentDialog.PrimaryButtonClick += async (s, e) =>
        {
            if (textBox.Text.Length == 0)
            {
                e.Cancel = true;
            }
            else
            {
                var newDiscussion = new DiscussItem()
                {
                    Id = Guid.NewGuid(),
                    Title = textBox.Text,
                    CreationTime = DateTime.Now,
                    Messages =
                    [],
                    ChatOptions = new()
                };
                _discussItems.Add(newDiscussion);
                var ndVM = new DiscussItemViewModel(newDiscussion);
                ndVM.PropertyChanged += OnDiscussItemPropertyChanged;
                DiscussItemViewModels.Add(ndVM);
            }
        };
        contentDialog.DefaultButton = ContentDialogButton.Primary;
        contentDialog.XamlRoot = Parent.Content.XamlRoot;
        await contentDialog.ShowAsync();
    }

    private void OnDiscussItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "IsViewed")
            DiscussionViewStatusChanged(this, new()
            {
                DiscussItem = (sender as DiscussItemViewModel).InnerObject,
                Status = (sender as DiscussItemViewModel).LeastStatus
            });
    }

    private Dictionary<string, Page> _pages = new();
    private string _currentPageId = string.Empty;
    public string CurrentPageId => _currentPageId;
    public void TryNavigate(string pageId, Func<Page> addPageFactory)
    {
        if (_pages.TryGetValue(pageId, out Page page))
        {
            page.RequestedTheme = Parent.RequestedTheme;
            ContentPage = page;
        }
        else
        {
            Page newPage = addPageFactory();
            newPage.RequestedTheme = Parent.RequestedTheme;
            _pages.Add(pageId, newPage);
            ContentPage = newPage;
        }
        _currentPageId = pageId;
        GC.Collect();
    }

    public void TryRemovePage(string pageId)
    {
        if (_pages.TryGetValue(pageId, out Page page))
        {
            if(pageId == _currentPageId)
            {
                ContentPage = null;
                _currentPageId = string.Empty;
            }
            _pages.Remove(pageId);
        }
    }
}
