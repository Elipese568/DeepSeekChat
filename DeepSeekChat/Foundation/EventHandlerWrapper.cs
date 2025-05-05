using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Foundation;

/// <summary>
/// A wrapper for event handlers that simplifies the process of adding, removing, and invoking multiple handlers, provided a simple implementation for multicast event.
/// </summary>
/// <typeparam name="THandler">The type of event handler, requires a delegate type</typeparam>
public class EventHandlerWrapper<THandler>
    where THandler : Delegate
{
    public static EventHandlerWrapper<THandler> Create() => new();

    private List<THandler> _handlers;
    private EventHandlerWrapper()
    {
        _handlers = new List<THandler>();
    }

    public void AddHandler(THandler handler)
    {
        if (!_handlers.Contains(handler))
            _handlers.Add(handler);
    }

    public void RemoveHandler(THandler handler)
    {
        _handlers.Remove(handler);
    }

    public void Invoke(params object[] parameters)
    {
        foreach (var handler in _handlers)
        {
            handler.Method.Invoke(handler.Target, parameters);
        }
    }
}
