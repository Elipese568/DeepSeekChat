using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeepSeekChat.Command;
using DeepSeekChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.ViewModels;

public partial class DiscussionViewModel : ObservableRecipient
{
    [ObservableProperty]
    private DiscussItem _selectedDiscussItem;

    private string _inputingPrompt;
    public string InputingPrompt
    {
        get => _inputingPrompt;
        set
        {
            SetProperty(ref _inputingPrompt, value);
            OnSendableChanging()
        }
    }

    [ObservableProperty]
    private bool _sendable;

    public SendCommand SendCommand { get; }

    public DiscussionViewModel()
    {
        SendCommand = new();
        SendCommand.CanExecuteChanged += (s, e) =>
        {
            Sendable = !(SendCommand.InProgress ^ SendCommand.CanExecute());
        }
    }

    public async Task SendPrompt()
    {
        _selectedDiscussItem.Messages.Add(new ChatMessage()
        {
            UserPrompt = _inputingPrompt,
            AiAnswer = "Thinking..."
        });
        InputingPrompt = "";
    }
}
