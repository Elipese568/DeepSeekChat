using DeepSeekChat.Models;
using DeepSeekChat.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.ViewModels;

public class ApplicationChatMessageViewModel : WrapperViewModelBase<ApplicationChatMessage>
{
	public ApplicationChatMessageViewModel(ApplicationChatMessage message) : base(message)
	{
		AiChatCompletion = new AiChatCompletionViewModel(message.AiChatCompletion);
	}

	public TokenUsage TokenUsage
	{
		get { return _innerObject.TokenUsage; }
		set
		{
			_innerObject.TokenUsage = value;
			OnPropertyChanged();
		}
	}

	public string UserPrompt
	{
		get { return _innerObject.UserPrompt; }
		set
		{
			_innerObject.UserPrompt = value;
		}
	}

	private AiChatCompletionViewModel _aiChatCompletionViewModel;

	public AiChatCompletionViewModel AiChatCompletion
	{
		get { return _aiChatCompletionViewModel; }
		set { _aiChatCompletionViewModel = value; }
	}


	public ChatCompletionMetadata Metadata
	{
		get { return _innerObject.CurrentMessageMetadata; }
		set
		{
			_innerObject.CurrentMessageMetadata = value;
		}
	}
	public string Id
	{
		get { return _innerObject.Id; }
		set
		{
			_innerObject.Id = value;
		}
	}
	public ProgressStatus ProgressStatus
	{
		get { return _innerObject.ProgressStatus; }
		set
		{
			_innerObject.ProgressStatus = value;
			OnPropertyChanged();
		}
	}
}
