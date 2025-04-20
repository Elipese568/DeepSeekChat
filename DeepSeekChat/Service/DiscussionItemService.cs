using DeepSeekChat.Models;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Core.Preview;

namespace DeepSeekChat.Service;

[JsonStorageFile(FileName = "chatHistory.json")]
public class DiscussionItemService : JsonSeriailizingServiceBase<List<DiscussionItem>>
{
    public DiscussionItemService() : base()
    {
    }
    public List<DiscussionItem> GetStroragedDiscussionItems()
    {
        return _data;
    }

    public DiscussionItem CreateNewDiscussionItem(string name)
    {
        var discussionItem = new DiscussionItem()
        {
            LeastStatus = ProgressStatus.None,
            ChatOptions = new(),
            Id = Guid.NewGuid(),
            CreationTime = DateTime.Now,
            IsViewed = true,
            Messages = new List<ApplicationChatMessage>(),
            Title = name
        };
        _data.Add(discussionItem);

        return discussionItem;
    }

    public DiscussionItem GetDiscussionItemById(Guid id)
    {
        return _data.FirstOrDefault(x => x.Id == id);
    }

    public void RemoveDiscussionItem(Guid id)
    {
        var discussionItem = _data.FirstOrDefault(x => x.Id == id);
        if (discussionItem != null)
        {
            _data.Remove(discussionItem);
        }
    }
}
