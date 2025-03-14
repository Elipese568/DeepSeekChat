using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    [ObservableProperty]
    private string _inputingPrompt;

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
