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


public class DiscussionItemService
{
    private StorageFile _historyStorage;
    private List<DiscussionItem> _discussionItems;

    public DiscussionItemService()
    {
        Initialize();
        AppDomain.CurrentDomain.ProcessExit += ChatHistoryService_CloseRequested;
    }

    private void ChatHistoryService_CloseRequested(object? sender, EventArgs e)
    {
        var serializedStream = _historyStorage.OpenStreamForWriteAsync().GetAwaiter().GetResult();
        JsonSerializer.Serialize(serializedStream, _discussionItems);
        serializedStream.Flush();
        serializedStream.Close();
    }

    private void Initialize()
    {
        var localFolder = ApplicationData.Current.LocalFolder;
        _historyStorage = localFolder.CreateFileAsync("chatHistory.json", CreationCollisionOption.OpenIfExists).GetAwaiter().GetResult();

        using var readStream = _historyStorage.OpenStreamForReadAsync().GetAwaiter().GetResult();
        if(readStream.Length == 0)
        {
            _discussionItems = new List<DiscussionItem>();
            return;
        }
        _discussionItems = JsonSerializer.DeserializeAsync<List<DiscussionItem>>(readStream).GetAwaiter().GetResult();
    }

    public List<DiscussionItem> GetStroragedDiscussionItems()
    {
        return _discussionItems;
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
        _discussionItems.Add(discussionItem);

        return discussionItem;
    }

    public DiscussionItem GetDiscussionItemById(Guid id)
    {
        return _discussionItems.FirstOrDefault(x => x.Id == id);
    }

    public void RemoveDiscussionItem(Guid id)
    {
        var discussionItem = _discussionItems.FirstOrDefault(x => x.Id == id);
        if (discussionItem != null)
        {
            _discussionItems.Remove(discussionItem);
        }
    }
}
