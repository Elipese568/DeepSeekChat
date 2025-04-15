using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.ViewModels;

public abstract class WrapperViewModelBase<TWarp> : ObservableRecipient
{
    protected TWarp _innerObject;
    public WrapperViewModelBase(TWarp wrapped)
    {
        _innerObject = wrapped;
        InnerObject = wrapped;
    }

    public readonly TWarp InnerObject;
}
