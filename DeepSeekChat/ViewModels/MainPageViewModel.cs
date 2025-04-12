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

namespace DeepSeekChat.ViewModels;

public partial class MainPageViewModel : ObservableRecipient
{
    [ObservableProperty]
    private ObservableCollection<DiscussItem> _discussItems = new();

    [ObservableProperty]
    private DiscussItem _operatingItem;

    [ObservableProperty]
    private DiscussItem _selectedDiscussItem;

    [ObservableProperty]
    private Page _contentPage;

    public MainPage Parent { get; set; }

    public MainPageViewModel(MainPage page)
    {
        Parent = page;
    }

    [RelayCommand]
    public void RemoveDiscussion()
    {
        DiscussItems.RemoveAt(DiscussItems.IndexOf(x => x.Id == OperatingItem.Id));
        TryRemovePage(OperatingItem.Id.ToString());
    }

    [RelayCommand]
    public async Task ChangeDiscussionTitle()
    {
        int operatingIndex = DiscussItems.IndexOf(x => x.Id == OperatingItem.Id);
        ContentDialog contentDialog = new();
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
                DiscussItems[operatingIndex].Title = textBox.Text;
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
                DiscussItems.Add(new DiscussItem()
                {
                    Id = Guid.NewGuid(),
                    Title = textBox.Text,
                    CreationTime = DateTime.Now,
                    Messages =
                    [],
                    ChatOptions = new()
                });
            }
        };
        contentDialog.DefaultButton = ContentDialogButton.Primary;
        contentDialog.XamlRoot = Parent.Content.XamlRoot;
        await contentDialog.ShowAsync();
    }

    private Dictionary<string, Page> _pages = new();
    private string _currentPageId = string.Empty;
    public string CurrentPageId => _currentPageId;
    public void TryNavigate(string pageId, Func<Page> addPageFactory)
    {
        if (_pages.TryGetValue(pageId, out Page page))
        {
            ContentPage = page;
        }
        else
        {
            Page newPage = addPageFactory();
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
