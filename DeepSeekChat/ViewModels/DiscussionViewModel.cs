using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeepSeekChat.Command;
using DeepSeekChat.Helper;
using DeepSeekChat.Helper.Converters;
using DeepSeekChat.Models;
using DeepSeekChat.Service;
using DeepSeekChat.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.ViewModels;

public partial class DiscussionViewModel : ObservableRecipient
{
    private readonly CallAICommand _sendCommand;
    private readonly SettingService _settingService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendCommand))]
    private string _inputingPrompt;
    

    public DiscussionItemViewModel SelectedDiscussItemViewModel { get; set; }

    public DiscussionViewModel(DiscussionItemViewModel item)
    {
        SelectedDiscussItemViewModel = item;
        _settingService = App.Current.GetService<SettingService>();

        _sendCommand = new CallAICommand(item.InnerObject);
        _sendCommand.StreamResponseReceived += OnStreamResponseReceived;
        _sendCommand.StreamCompleted += OnStreamCompleted;
        _sendCommand.CompletionMetadataReceived += OnCompletionMetadataReceived;
        if(!string.IsNullOrWhiteSpace(_settingService.Read(SettingService.SETTING_APIKEY)))
        {
            try
            {
                _sendCommand.Configure(_settingService.Read(SettingService.SETTING_APIKEY), App.Current.GetService<ModelsManagerService>().GetModelById(new Guid(_settingService.Read(SettingService.SETTING_SELECTED_MODEL))).ModelID);
            }
            catch { }
        }

        _settingService.SettingChanged += OnSettingChanged;
    }

    private void OnSettingChanged(object? sender, SettingChangedEventArgs e)
    {
        if(e.Name is not SettingService.SETTING_APIKEY and not SettingService.SETTING_SELECTED_MODEL)
            return;

        if (string.IsNullOrWhiteSpace(e.Value))
            return;

        _sendCommand.Configure(_settingService.Read(SettingService.SETTING_APIKEY), App.Current.GetService<ModelsManagerService>().GetModelById(new Guid(_settingService.Read(SettingService.SETTING_SELECTED_MODEL))).ModelID);
    }

    private void OnCompletionMetadataReceived(object? sender, ChatCompletionMetadata e)
    {
        SelectedDiscussItemViewModel.MessagesViewModel.MessageViewModels[^1].Metadata = e;
    }

    [RelayCommand(CanExecute = nameof(CanSend))]
    private async Task Send(string prompt)
    {
        if (string.IsNullOrWhiteSpace(InputingPrompt)) return;

        InputingPrompt = string.Empty;
        SelectedDiscussItemViewModel.MessagesViewModel.Add(new ApplicationChatMessage
        {
            UserPrompt = prompt,
            AiChatCompletion = new()
            {
                ReasoningContent = "",
                Content = ""
            },
            TokenUsage = new(),
            ProgressStatus = ProgressStatus.InProgress
        });

        _sendCommand.Execute(prompt);
    }

    [RelayCommand]
    public async Task DetailEditSystemPrompt()
    {
        ContentDialog contentDialog = new();
        contentDialog.RequestedTheme = (MainWindow.Current.Content as FrameworkElement).RequestedTheme;
        contentDialog.Title = "Edit System Prompt";
        contentDialog.PrimaryButtonText = "Confirm";
        contentDialog.SecondaryButtonText = "Cancel";
        TextBox textBox = new()
        {
            MaxLength = int.MaxValue,
            AcceptsReturn = true,
            HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch,
            VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Stretch,
            TextWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap
        };
        ScrollViewer.SetVerticalScrollBarVisibility(textBox, ScrollBarVisibility.Visible);
        textBox.Text = SelectedDiscussItemViewModel.ChatOptionsViewModel.SystemPrompt;
        contentDialog.Content = textBox;
        contentDialog.XamlRoot = MainPage.Current.Content.XamlRoot;
        if (await contentDialog.ShowAsync() == ContentDialogResult.Primary)
        {
            SelectedDiscussItemViewModel.ChatOptionsViewModel.SystemPrompt = textBox.Text;
        }
    }

    private bool CanSend() => !string.IsNullOrWhiteSpace(InputingPrompt) && !_sendCommand.IsRunning;

    [RelayCommand]
    public void RandomSeed()
    {
        SelectedDiscussItemViewModel.ChatOptionsViewModel.Seed = Random.Shared.Next();
    }

    public void StopGenerating()
    {
        _sendCommand.Cancel();
        SelectedDiscussItemViewModel.MessagesViewModel.MessageViewModels[^1].ProgressStatus = ProgressStatus.Stoped;
    }
    private void OnStreamResponseReceived(object sender, ChatResponseReceivedEventArgs e)
    {
        if (e.Type == UpdateType.Reasoning)
        {
            SelectedDiscussItemViewModel.MessagesViewModel.MessageViewModels[^1].AiChatCompletion.ReasoningContent += e.ContentUpdate;
        }
        else
        {
            SelectedDiscussItemViewModel.MessagesViewModel.MessageViewModels[^1].AiChatCompletion.Content += e.ContentUpdate.TrimStart('\n');
        }
        SelectedDiscussItemViewModel.MessagesViewModel.MessageViewModels[^1].TokenUsage = e.TokenUsage;
    }

    private void OnStreamCompleted(object sender, ChatResponseCompletedEventArgs e)
    {
        if(SelectedDiscussItemViewModel.MessagesViewModel.MessageViewModels[^1].ProgressStatus != ProgressStatus.Stoped)
            SelectedDiscussItemViewModel.MessagesViewModel.MessageViewModels[^1].ProgressStatus = e.Status;
        SelectedDiscussItemViewModel.LeastStatus = e.Status;
        if (MainPage.Current.ViewModel.SelectedDiscussItem.Id != SelectedDiscussItemViewModel.Id)
            SelectedDiscussItemViewModel.IsViewed = false;
        else
            SelectedDiscussItemViewModel.IsViewed = true;
    }

    public event EventHandler ScrollToBottomRequested;
}
