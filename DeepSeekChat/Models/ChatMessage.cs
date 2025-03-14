using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI;

namespace DeepSeekChat.Models;

public partial class ChatMessage : ObservableObject
{
    [ObservableProperty]
    private string _userPrompt;

    [ObservableProperty]
    private string _aiAnswer;
}
