using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeepSeekChat.Models;

public partial class DiscussItem
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public DateTime CreationTime { get; set; }

    public List<ApplicationChatMessage> Messages { get; set; }

    public ChatOptions ChatOptions { get; set; }

    public ProgressStatus LeastStatus { get; set; }

    public bool IsViewed { get; set; } = true;
}
