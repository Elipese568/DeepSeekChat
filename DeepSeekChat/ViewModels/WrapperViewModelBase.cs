using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.ViewModels;

public abstract class WrapperViewModelBase<TWrap> : ObservableRecipient
{
    protected TWrap _innerObject;
    public WrapperViewModelBase(TWrap wrapped)
    {
        _innerObject = wrapped;
        InnerObject = wrapped;
    }

    public readonly TWrap InnerObject;
}
