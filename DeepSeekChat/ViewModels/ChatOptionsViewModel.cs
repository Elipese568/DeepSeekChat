using DeepSeekChat.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.ViewModels;

public class ChatOptionsViewModel : WrapperViewModelBase<ChatOptionsModel>
{
    public ChatOptionsViewModel(ChatOptionsModel wrapped) : base(wrapped)
    {

    }
    public string SystemPrompt
    {
        get { return _innerObject.SystemPrompt; }
        set
        {
            _innerObject.SystemPrompt = value;
            OnPropertyChanged();
        }
    }
    public float Temperature
    {
        get { return _innerObject.Temperature; }
        set
        {
            _innerObject.Temperature = value;
            OnPropertyChanged();
        }
    }
    public int MaxTokens
    {
        get { return _innerObject.MaxTokens; }
        set
        {
            _innerObject.MaxTokens = value;
            OnPropertyChanged();
        }
    }
    public float TopP
    {
        get { return _innerObject.TopP; }
        set
        {
            _innerObject.TopP = value;
            OnPropertyChanged();
        }
    }
    public float FrequencyPenalty
    {
        get { return _innerObject.FrequencyPenalty; }
        set
        {
            _innerObject.FrequencyPenalty = value;
            OnPropertyChanged();
        }
    }
    public int Seed
    {
        get { return _innerObject.Seed; }
        set
        {
            _innerObject.Seed = value;
            OnPropertyChanged();
        }
    }
    public bool SeedAutoRefresh
    {
        get { return _innerObject.SeedAutoRefresh; }
        set
        {
            _innerObject.SeedAutoRefresh = value;
            OnPropertyChanged();
        }
    }

    public int TopK
    {
        get { return _innerObject.TopK; }
        set
        {
            _innerObject.TopK = value;
            OnPropertyChanged();
        }
    }

    public int ThinkingBudget
    {
        get { return _innerObject.ThinkingBudget; }
        set
        {
            _innerObject.ThinkingBudget = value;
            OnPropertyChanged();
        }
    }

}
