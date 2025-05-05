using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Service;

public interface IImplementationLifetimeService
{
    protected virtual void OnInitialize() { }
    protected virtual void OnDispose() { }
}

public class ImplementationLifetimeServiceBase : IImplementationLifetimeService
{
    protected virtual void OnInitialize() { }
    protected virtual void OnDispose() { }

    public ImplementationLifetimeServiceBase()
    {
        OnInitialize();
        App.Current.ExitProcess += (s, e) =>
        {
            OnDispose();
        };
    }
}
