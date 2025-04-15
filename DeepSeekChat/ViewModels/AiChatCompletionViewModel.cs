using DeepSeekChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.ViewModels;

public class AiChatCompletionViewModel : WrapperViewModelBase<AiChatCompletion>
{
    public AiChatCompletionViewModel(AiChatCompletion wrapped) : base(wrapped) { }

	public string ReasoningContent
	{
		get { return _innerObject.ReasoningContent; }
		set
		{
			_innerObject.ReasoningContent = value;
			OnPropertyChanged();
		}
	}

	public string Content
	{
		get { return _innerObject.Content; }
		set
		{
			_innerObject.Content = value;
			OnPropertyChanged();
		}
	}
}
