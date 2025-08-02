using DeepSeekChat.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.ViewModels;

public class ContentPartViewModel : WrapperViewModelBase<ContentPart>
{
    public ContentPartViewModel(ContentPart wrapped) : base(wrapped) { }

    public string Type
    {
        get { return _innerObject.Type; }
    }

    public static ContentPartViewModel Create(ContentPart contentPart)
    {
        return contentPart switch
        {
            TextContentPart textContentPart => new TextContentPartViewModel(textContentPart),
            ToolCallingContentPart toolCallingContentPart => new ToolCallingContentPartViewModel(toolCallingContentPart),
            _ => new ContentPartViewModel(contentPart)
        };
    }
}

public class TextContentPartViewModel : ContentPartViewModel
{
    public TextContentPartViewModel(TextContentPart wrapped) : base(wrapped) { }
    public string Text
    {
        get { return ((TextContentPart)_innerObject).Text; }
        set
        {
            ((TextContentPart)_innerObject).Text = value;
            OnPropertyChanged();
        }
    }
}

public class ToolCallingContentPartViewModel : ContentPartViewModel
{
    public ToolCallingContentPartViewModel(ToolCallingContentPart wrapped) : base(wrapped) { }
    public string Name
    {
        get { return ((ToolCallingContentPart)_innerObject).Name; }
        set
        {
            ((ToolCallingContentPart)_innerObject).Name = value;
            OnPropertyChanged();
        }
    }

    public Dictionary<string, string> Arguments
    {
        get { return ((ToolCallingContentPart)_innerObject).Arguments; }
        set
        {
            ((ToolCallingContentPart)_innerObject).Arguments = value;
            OnPropertyChanged();
        }
    }

    public string Result
    {
        get { return ((ToolCallingContentPart)_innerObject).Result; }
        set
        {
            ((ToolCallingContentPart)_innerObject).Result = value;
            OnPropertyChanged();
        }
    }
}



public class AiChatCompletionViewModel : WrapperViewModelBase<AiChatCompletion>
{
    public AiChatCompletionViewModel(AiChatCompletion wrapped) : base(wrapped)
    {
        ContentViewModels = new(wrapped.Content?.Select(ContentPartViewModel.Create));
    }

    public void AddContentViewModel(ContentPartViewModel contentPartViewModel)
    {
        ContentViewModels.Add(contentPartViewModel);
        _innerObject.Content?.Add(contentPartViewModel.InnerObject);
    }

    public void RemoveContentViewModel(ContentPartViewModel contentPartViewModel)
    {
        ContentViewModels.Remove(contentPartViewModel);
        _innerObject.Content?.Remove(contentPartViewModel.InnerObject);
    }

    public string ReasoningContent
    {
        get { return _innerObject.ReasoningContent; }
        set
        {
            _innerObject.ReasoningContent = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<ContentPartViewModel> ContentViewModels { get; set; }
}
