using DeepSeekChat.Models;
using DeepSeekChat.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepSeekChat.Service;
using Microsoft.UI.Xaml.Media;

namespace DeepSeekChat.ViewModels;

public class ApplicationChatMessageViewModel : WrapperViewModelBase<ApplicationChatMessage>
{
	public ApplicationChatMessageViewModel(ApplicationChatMessage message) : base(message)
	{
		AiChatCompletion = new AiChatCompletionViewModel(message.AiChatCompletion);
		App.Current.GetService<AvatarManagerService>().SelectedUserAvatarChanged += (s, e) =>
		{
			OnPropertyChanged(e.Type.HasFlag(AvatarType.User) ? nameof(UserAvatar) : nameof(AiAvatar));
		};
        OnPropertyChanged(nameof(UserAvatar));
        OnPropertyChanged(nameof(AiAvatar));
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
			OnPropertyChanged();
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

	public ImageSource AiAvatar => App.Current.GetService<AvatarManagerService>().GetSelectedAiAvatarViewModel().ImageSource;
    public ImageSource UserAvatar => App.Current.GetService<AvatarManagerService>().GetSelectedUserAvatarViewModel().ImageSource;
}
