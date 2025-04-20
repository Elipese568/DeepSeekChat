using DeepSeekChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.ViewModels;

public class AiModelViewModel : WrapperViewModelBase<AiModel>
{
    public AiModelViewModel(AiModel wrapped) : base(wrapped)
    {
    }

    public string Name
    {
        get { return _innerObject.Name; }
        set
        {
            _innerObject.Name = value;
            OnPropertyChanged();
        }
    }
    public string Description
    {
        get { return _innerObject.Description; }
        set
        {
            _innerObject.Description = value;
            OnPropertyChanged();
        }
    }
    public string ModelID
    {
        get { return _innerObject.ModelID; }
        set
        {
            _innerObject.ModelID = value;
            OnPropertyChanged();
        }
    }

    public Guid UniqueID => _innerObject.UniqueID;
}
