using DeepSeekChat.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.ViewModels;

public class DiscussionItemViewModel : WrapperViewModelBase<DiscussionItem>
{
    public DiscussionItemViewModel(DiscussionItem wrapped) : base(wrapped)
    {
        _chatOptionsViewModel = new ChatOptionsViewModel(wrapped.ChatOptions);
		_messagesViewModel = new MessagesViewModel(wrapped.Messages);
    }

	public string Title
	{
		get { return _innerObject.Title; }
		set
		{
			_innerObject.Title = value;
			OnPropertyChanged();
		}
	}

	private MessagesViewModel _messagesViewModel;

	public MessagesViewModel MessagesViewModel
	{
		get { return _messagesViewModel; }
		set { _messagesViewModel = value; }
	}


	public ProgressStatus LeastStatus
    {
		get { return _innerObject.LeastStatus; }
		set
		{
			_innerObject.LeastStatus = value;
			OnPropertyChanged();
		}
	}

    public bool IsViewed
    {
        get { return _innerObject.IsViewed; }
        set
        {
            _innerObject.IsViewed = value;
            OnPropertyChanged();
        }
    }

    public Guid Id
	{
		get { return _innerObject.Id; }
		set
		{
			_innerObject.Id = value;
		}
	}

	public DateTime CreationTime
	{
		get { return _innerObject.CreationTime; }
		set
		{
			_innerObject.CreationTime = value;
		}
	}

	private ChatOptionsViewModel _chatOptionsViewModel;

	public ChatOptionsViewModel ChatOptionsViewModel
	{
		get { return _chatOptionsViewModel; }
		set { _chatOptionsViewModel = value; }
	}

}
