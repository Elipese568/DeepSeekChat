using System;
using System.Collections.Generic;

namespace DeepSeekChat.Core.Models;

public partial class DiscussionItemModel
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public DateTime CreationTime { get; set; }

    public List<ApplicationChatMessageModel> Messages { get; set; }

    public ChatOptionsModel ChatOptions { get; set; }

    public ProgressStatus LeastStatus { get; set; }

    public bool IsViewed { get; set; }
}
