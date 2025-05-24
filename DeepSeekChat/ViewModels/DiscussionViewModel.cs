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
    private readonly ExecuteAICommand _sendCommand;
    private readonly SettingService _settingService;
    private readonly AvatarManagerService _avatarManagerService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendCommand))]
    private string _inputingPrompt;

    [ObservableProperty]
    private AvatarDataViewModel _userAvatarDataViewModel;
    [ObservableProperty]
    private AvatarDataViewModel _aiAvatarDataViewModel;

    public DiscussionItemViewModel SelectedDiscussItemViewModel { get; set; }

    public DiscussionViewModel(DiscussionItemViewModel item)
    {
        SelectedDiscussItemViewModel = item;
        _settingService = App.Current.GetService<SettingService>();
        _avatarManagerService = App.Current.GetService<AvatarManagerService>();

        _sendCommand = new ExecuteAICommand(item.InnerObject);
        _sendCommand.StreamResponseReceived += OnStreamResponseReceived;
        _sendCommand.StreamCompleted += OnStreamCompleted;
        _sendCommand.CompletionMetadataReceived += OnCompletionMetadataReceived;

        UserAvatarDataViewModel = _avatarManagerService.GetSelectedUserAvatarViewModel();
        AiAvatarDataViewModel = _avatarManagerService.GetSelectedAiAvatarViewModel();

        _avatarManagerService.SelectedUserAvatarChanged += (s, e) =>
        {
            if (e.Type.HasFlag(AvatarType.User))
                UserAvatarDataViewModel = e.ViewModel;
            else
                AiAvatarDataViewModel = e.ViewModel;
        };
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

        if (SelectedDiscussItemViewModel.ChatOptionsViewModel.SeedAutoRefresh)
            RandomSeed();

        _sendCommand.Execute(prompt);
    }

    [RelayCommand]
    public async Task DetailEditSystemPrompt()
    {
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
        var contentDialog = ContentDialogHelper.CreateContentDialog(
            "EditSystemPromptDialogHeader".GetLocalized("DiscussionPage"),
            textBox,
            "ConfirmText".GetLocalized(),
            "CancelText".GetLocalized(),
            null,
            ContentDialogButton.Primary,
            MainPage.Current.Content.XamlRoot);
        if (await contentDialog.ShowAsync() == ContentDialogResult.Primary)
        {
            SelectedDiscussItemViewModel.ChatOptionsViewModel.SystemPrompt = textBox.Text;
        }
    }


    private bool CanSend() => !string.IsNullOrWhiteSpace(InputingPrompt) && _sendCommand.CanExecute(null);

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
