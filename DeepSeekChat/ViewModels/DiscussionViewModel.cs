using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeepSeekChat.Command;
using DeepSeekChat.Helper;
using DeepSeekChat.Helper.Converters;
using DeepSeekChat.Models;
using DeepSeekChat.Views;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.ViewModels;


public partial class DiscussionViewModel : ObservableRecipient
{
    public readonly CallAICommand _sendCommand;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendCommand))]
    private string _inputingPrompt;

    [ObservableProperty]
    private DiscussItem _selectedDiscussItem;

    public DiscussionViewModel(DiscussItem item)
    {
        _selectedDiscussItem = item;
        _sendCommand = new CallAICommand(SettingHelper.Read("ApiKey", string.Empty), _selectedDiscussItem);
        _sendCommand.StreamResponseReceived += OnStreamResponseReceived;
        _sendCommand.StreamCompleted += OnStreamCompleted;
        _sendCommand.CompletionMetadataReceived += OnCompletionMetadataReceived;
    }

    private void OnCompletionMetadataReceived(object? sender, ChatCompletionMetadata e)
    {
        SelectedDiscussItem.Messages[^1].CurrentMessageMetadata = e;
    }

    [RelayCommand(CanExecute = nameof(CanSend))]
    private async Task Send(string prompt)
    {
        if (string.IsNullOrWhiteSpace(InputingPrompt)) return;

        InputingPrompt = string.Empty;
        SelectedDiscussItem.Messages.Add(new ApplicationChatMessage
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
        textBox.Text = SelectedDiscussItem.ChatOptions.SystemPrompt;
        contentDialog.Content = textBox;
        contentDialog.XamlRoot = MainPage.Current.Content.XamlRoot;
        if (await contentDialog.ShowAsync() == ContentDialogResult.Primary)
        {
            SelectedDiscussItem.ChatOptions.SystemPrompt = textBox.Text;
        }
    }

    private bool CanSend() => !string.IsNullOrWhiteSpace(InputingPrompt) && !_sendCommand.IsRunning;

    [RelayCommand]
    public void RandomSeed()
    {
        SelectedDiscussItem.ChatOptions.Seed = Random.Shared.Next();
    }

    public void StopGenerating()
    {
        _sendCommand.Cancel();
        SelectedDiscussItem.Messages[^1].ProgressStatus = ProgressStatus.Stoped;
    }
    private void OnStreamResponseReceived(object sender, ChatResponseReceivedEventArgs e)
    {
        if (e.Type == UpdateType.Reasoning)
        {
            SelectedDiscussItem.Messages[^1].AiChatCompletion.ReasoningContent += e.ContentUpdate;
        }
        else
        {
            SelectedDiscussItem.Messages[^1].AiChatCompletion.Content += e.ContentUpdate.TrimStart('\n');
        }
        SelectedDiscussItem.Messages[^1].TokenUsage = e.TokenUsage;
    }

    private void OnStreamCompleted(object sender, ChatResponseCompletedEventArgs e)
    {
        if(SelectedDiscussItem.Messages[^1].ProgressStatus != ProgressStatus.Stoped)
            SelectedDiscussItem.Messages[^1].ProgressStatus = e.Status;
        if(MainPage.Current.ViewModel.SelectedDiscussItem.Id != SelectedDiscussItem.Id)
            SelectedDiscussItem.CurrentUIStatus = e.Status;
    }

    public event EventHandler ScrollToBottomRequested;
}
