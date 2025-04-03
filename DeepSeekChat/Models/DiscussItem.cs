using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeepSeekChat.Models;

public partial class DiscussItem : ObservableObject
{
    [ObservableProperty]
    private Guid _id;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private DateTime _creationTime;

    [ObservableProperty]
    private ObservableCollection<ApplicationChatMessage> _messages;

    [ObservableProperty]
    private ChatOptions _chatOptions;

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    [ObservableProperty]
    private ProgressStatus _currentUIStatus = ProgressStatus.None;
}
