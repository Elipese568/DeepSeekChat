using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeepSeekChat.Command;
using DeepSeekChat.Models;
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
        _sendCommand = new CallAICommand("sk-pxynejtfvsigjxldrkivxfugfbxggqzrauumsirdlrvifvcy", _selectedDiscussItem);
        _sendCommand.StreamResponseReceived += OnStreamResponseReceived;
        _sendCommand.StreamCompleted += OnStreamCompleted;
    }

    [RelayCommand(CanExecute = nameof(CanSend))]
    private async Task Send(string prompt)
    {
        if (string.IsNullOrWhiteSpace(InputingPrompt)) return;

        InputingPrompt = string.Empty;
        SelectedDiscussItem.Messages.Add(new ApplicationChatMessage
        {
            UserPrompt = prompt,
            AiAnswer = "Thinking..."
        });

        _sendCommand.Execute(prompt);
    }

    private bool CanSend() => !string.IsNullOrWhiteSpace(InputingPrompt) && !_sendCommand.IsRunning;

    private void OnStreamResponseReceived(object sender, string response)
    {
        var lastMessage = SelectedDiscussItem.Messages.LastOrDefault();
        if (lastMessage != null)
        {
            lastMessage.AiAnswer = response;
        }
    }

    private void OnStreamCompleted(object sender, EventArgs e)
    {

    }

    public event EventHandler ScrollToBottomRequested;
}
