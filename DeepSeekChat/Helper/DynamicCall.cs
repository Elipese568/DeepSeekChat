using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Helper;

public static class DynamicCall
{
    public static Func<TOut> GetInvoker<T, TOut>(T obj, string methodName)
    {
        var type = obj.GetType();
        var method = type.GetMethod(methodName);
        if (method != null)
        {
            return () => (TOut)method.Invoke(obj, null);
        }
        else
        {
            throw new Exception($"Method {methodName} not found in type {type.Name}");
        }
    }

    public static Func<TIn1,TOut> GetInvoker<TObj, TIn1, TOut>(TObj obj, string methodName)
    {
        var type = obj.GetType();
        var method = type.GetMethod(methodName);
        if (method != null)
        {
            return (p1) => (TOut)method.Invoke(obj, [p1]);
        }
        else
        {
            throw new Exception($"Method {methodName} not found in type {type.Name}");
        }
    }

    public static Func<TIn1, TIn2, TOut> GetInvoker<TObj, TIn1, TIn2, TOut>(TObj obj, string methodName)
    {
        var type = obj.GetType();
        var method = type.GetMethod(methodName);
        if (method != null)
        {
            return (p1, p2) => (TOut)method.Invoke(obj, [p1, p2]);
        }
        else
        {
            throw new Exception($"Method {methodName} not found in type {type.Name}");
        }
    }

    public static Func<TIn1, TIn2, TIn3, TOut> GetInvoker<TObj, TIn1, TIn2, TIn3, TOut>(TObj obj, string methodName)
    {
        var type = obj.GetType();
        var method = type.GetMethod(methodName);
        if (method != null)
        {
            return (p1, p2, p3) => (TOut)method.Invoke(obj, [p1, p2, p3]);
        }
        else
        {
            throw new Exception($"Method {methodName} not found in type {type.Name}");
        }
    }

    public static Func<TIn1, TIn2, TIn3, TIn4, TOut> GetInvoker<TObj, TIn1, TIn2, TIn3, TIn4, TOut>(TObj obj, string methodName)
    {
        var type = obj.GetType();
        var method = type.GetMethod(methodName);
        if (method != null)
        {
            return (p1, p2, p3, p4) => (TOut)method.Invoke(obj, [p1, p2, p3, p4]);
        }
        else
        {
            throw new Exception($"Method {methodName} not found in type {type.Name}");
        }
    }

    public static Action GetVoidInvoker<T>(T obj, string methodName)
    {
        var type = obj.GetType();
        var method = type.GetMethod(methodName);
        if (method != null)
        {
            return () => { method.Invoke(obj, null); };
        }
        else
        {
            throw new Exception($"Method {methodName} not found in type {type.Name}");
        }
    }

    public static Action<TIn1> GetVoidInvoker<TObj, TIn1>(TObj obj, string methodName)
    {
        var type = obj.GetType();
        var method = type.GetMethod(methodName);
        if (method != null)
        {
            return (p1) => { method.Invoke(obj, [p1]); };
        }
        else
        {
            throw new Exception($"Method {methodName} not found in type {type.Name}");
        }
    }

    public static Action<TIn1, TIn2> GetVoidInvoker<TObj, TIn1, TIn2>(TObj obj, string methodName)
    {
        var type = obj.GetType();
        var method = type.GetMethod(methodName);
        if (method != null)
        {
            return (p1, p2) => { method.Invoke(obj, [p1, p2]); };
        }
        else
        {
            throw new Exception($"Method {methodName} not found in type {type.Name}");
        }
    }

    public static Action<TIn1, TIn2, TIn3> GetVoidInvoker<TObj, TIn1, TIn2, TIn3>(TObj obj, string methodName)
    {
        var type = obj.GetType();
        var method = type.GetMethod(methodName);
        if (method != null)
        {
            return (p1, p2, p3) => { method.Invoke(obj, [p1, p2, p3]); };
        }
        else
        {
            throw new Exception($"Method {methodName} not found in type {type.Name}");
        }
    }

    public static Action<TIn1, TIn2, TIn3, TIn4> GetVoidInvoker<TObj, TIn1, TIn2, TIn3, TIn4>(TObj obj, string methodName)
    {
        var type = obj.GetType();
        var method = type.GetMethod(methodName);
        if (method != null)
        {
            return (p1, p2, p3, p4) => { method.Invoke(obj, [p1, p2, p3, p4]); };
        }
        else
        {
            throw new Exception($"Method {methodName} not found in type {type.Name}");
        }
    }
}
