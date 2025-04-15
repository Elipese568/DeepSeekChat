using DeepSeekChat.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.ViewModels;

public class MessagesViewModel : WrapperViewModelBase<List<ApplicationChatMessage>>
{
    public MessagesViewModel(List<ApplicationChatMessage> wrapped) : base(wrapped)
    {
        MessageViewModels = new(wrapped.Select(x => new ApplicationChatMessageViewModel(x)));
    }

    private ObservableCollection<ApplicationChatMessageViewModel> _wrapedViewModels;

    public ObservableCollection<ApplicationChatMessageViewModel> MessageViewModels
    {
        get { return _wrapedViewModels; }
        set
        {
            _wrapedViewModels = value;
        }
    }

    public void Add(ApplicationChatMessage message)
    {
        _innerObject.Add(message);
        _wrapedViewModels.Add(new(message));
    }

    public void Remove(ApplicationChatMessage message)
    {
        _innerObject.Remove(message);
        _wrapedViewModels.Remove(_wrapedViewModels.FirstOrDefault(m => m.Id == message.Id));
    }
}
