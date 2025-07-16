using DeepSeekChat.Core.AI.Internal.Request;
using DeepSeekChat.Core.Network.Body;
using DeepSeekChat.Core.Network.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Core.AI.Chat.Message;

public class ChatMessageCollection : IList<ChatMessage>, IRequestBodyCollectionConvertable<MessageRequestBody>
{
    private readonly List<ChatMessage> _messages = new List<ChatMessage>();

    public ChatMessageCollection() { }
    public ChatMessageCollection(IEnumerable<ChatMessage> messages)
    {
        if (messages == null) throw new ArgumentNullException(nameof(messages), "Messages collection cannot be null.");
        _messages.AddRange(messages);
    }

    public ChatMessage this[int index] { get => _messages[index]; set => _messages[index] = value; }
    public int Count => _messages.Count;
    public bool IsReadOnly => false;
    public void Add(ChatMessage item) => _messages.Add(item);
    public void Clear() => _messages.Clear();
    public bool Contains(ChatMessage item) => _messages.Contains(item);
    public void CopyTo(ChatMessage[] array, int arrayIndex) => _messages.CopyTo(array, arrayIndex);
    public IEnumerator<ChatMessage> GetEnumerator() => _messages.GetEnumerator();
    public int IndexOf(ChatMessage item) => _messages.IndexOf(item);
    public void Insert(int index, ChatMessage item) => _messages.Insert(index, item);
    public bool Remove(ChatMessage item) => _messages.Remove(item);
    public void RemoveAt(int index) => _messages.RemoveAt(index);

    public MessageRequestBody[] ToRequestBodys()
    {
        return [.. _messages.Select(x => x.ToRequestBody())];
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}
