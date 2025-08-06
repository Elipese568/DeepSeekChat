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
using DeepSeekChat.Service;
using Microsoft.UI.Dispatching;
using CommunityToolkit.WinUI.Helpers;
using System.Net.NetworkInformation;

namespace DeepSeekChat.ViewModels;

public class DiscussionViewStatusChangedEventArgs : EventArgs
{
    public ProgressStatus Status { get; set; }
    public DiscussionItem DiscussItem { get; set; }
}

public partial class MainPageViewModel : ObservableRecipient
{
    private readonly DiscussionItemService _discussionItemService;
    private readonly SettingService _settingService;

    private List<DiscussionItem> _discussionItems;

    [ObservableProperty]
    private ObservableCollection<DiscussionItemViewModel> _discussionItemViewModels = new();

    [ObservableProperty]
    private DiscussionItemViewModel _operatingItem;

    [ObservableProperty]
    private DiscussionItemViewModel _selectedDiscussItem;

    [ObservableProperty]
    private Page _contentPage;

    [ObservableProperty]
    private bool _isApiKeyEmpty = false;

    [ObservableProperty]
    private bool _isApiKeyAvailable = true;

    [ObservableProperty]
    private bool _isServiceAvailable = true;

    public event EventHandler<DiscussionViewStatusChangedEventArgs> DiscussionViewStatusChanged;

    public MainPage Parent { get; set; }

    public MainPageViewModel(MainPage page)
    {
        Parent = page;

        _discussionItemService = App.Current.GetService<DiscussionItemService>();
        _settingService = App.Current.GetService<SettingService>();

        _discussionItems = _discussionItemService.GetStroragedDiscussionItems();

        DiscussionItemViewModels = new(_discussionItems.Select(x => new DiscussionItemViewModel(x)));

        NetworkHelper.Instance.NetworkChanged += (s, e) =>
        {
            Parent.DispatcherQueue.TryEnqueue(() =>
            {
                SettingStatusToDisplay(null, null);
            });
        };

        SettingStatusToDisplay(null, null);
        App.Current.GetService<SettingService>().SettingChanged += SettingStatusToDisplay;
        _discussionItemService.ItemChanged += (s, e) =>
        {
            if (e.Operation == ChangeOperation.Remove)
            {
                RemoveDiscussionItem(e.Item);
            }
        };
    }

    public async void SettingStatusToDisplay(object sender, SettingChangedEventArgs e)
    {
        if (!NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable || CheckServiceIsNotAvailable())
        {
            IsServiceAvailable = false;
            return;
        }
        else
        {
            IsServiceAvailable = true;
        }

        if (_settingService.Read(SettingService.SETTING_APIKEY) is string apikey && !string.IsNullOrWhiteSpace(apikey))
        {
            IsApiKeyEmpty = false;
            if (await App.Current.GetService<ClientService>().IsApiKeyVaildAsync(apikey))
            {
                IsApiKeyAvailable = true;
            }
            else
            {
                IsApiKeyAvailable = false;
            }
        }
        else
        {
            IsApiKeyEmpty = true;
            IsApiKeyAvailable = false;
        }
    }

    private bool CheckServiceIsNotAvailable()
    {
        try
        {
            using var ping = new Ping();
            return ping.Send(new Uri(_settingService.Read(SettingService.SETTING_SERVER_ENDPOINT, "https://api.deepseek.com")).Host).Status != IPStatus.Success;
        }
        catch { return true; }
    }

    private void RemoveDiscussionItem(DiscussionItem item)
    {
        DiscussionItemViewModels.Remove(DiscussionItemViewModels.FirstOrDefault(x => x.Id == item.Id));
    }

    [RelayCommand]
    public void RemoveDiscussion()
    {
        _discussionItemService.RemoveDiscussionItem(OperatingItem.InnerObject.Id);
    }

    [RelayCommand]
    public async Task ChangeDiscussionTitle()
    {
        int operatingIndex = _discussionItems.IndexOf(x => x.Id == OperatingItem.Id);
        TextBox textBox = new()
        {
            Header = "InputTitleTextBox.Header".GetLocalized("MainPage"),
            PlaceholderText = "InputTitleTextBox.PlaceholderText".GetLocalized("MainPage"),
            MaxLength = 32,
            Text = DiscussionItemViewModels[operatingIndex].Title,
            SelectionStart = DiscussionItemViewModels[operatingIndex].Title.Length
        };
        await ContentDialogHelper.ShowContentDialog(
            "ChangeTitleMenuItem.Text".GetLocalized("MainPage"),
            textBox,
            "ConfirmText".GetLocalized(),
            "CancelText".GetLocalized(),
            null,
            ContentDialogButton.Primary,
            Parent.Content.XamlRoot,
            async (s, e) =>
            {
                if (textBox?.Text.Length == 0)
                {
                    e.Cancel = true;
                }
                else
                {
                    DiscussionItemViewModels[operatingIndex].Title = textBox.Text;
                }
            });
    }

    [RelayCommand]
    public async Task AddDiscussion()
    {
        TextBox textBox = new()
        {
            Header = "InputTitleTextBox.Header".GetLocalized("MainPage"),
            PlaceholderText = "InputTitleTextBox.PlaceholderText".GetLocalized("MainPage"),
            MaxLength = 32
        };

        await ContentDialogHelper.ShowContentDialog(
            "AddDiscussionButton.Content".GetLocalized("MainPage"),
            textBox,
            primaryButtonText: "AddText".GetLocalized(),
            "CancelText".GetLocalized(),
            null,
            ContentDialogButton.Primary,
            Parent.Content.XamlRoot,
            async (s, e) =>
            {
                if (textBox.Text.Length == 0)
                {
                    e.Cancel = true;
                }
                else
                {
                    var current = _discussionItemService.CreateNewDiscussionItem(textBox.Text);
                    var ndVM = new DiscussionItemViewModel(current);
                    ndVM.PropertyChanged += OnDiscussItemPropertyChanged;
                    DiscussionItemViewModels.Add(ndVM);
                }
            });
    }

    private void OnDiscussItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "IsViewed")
            DiscussionViewStatusChanged?.Invoke(this, new()
            {
                DiscussItem = (sender as DiscussionItemViewModel).InnerObject,
                Status = (sender as DiscussionItemViewModel).LeastStatus
            });
    }

    private bool _doLeaveDestory = false;
}
