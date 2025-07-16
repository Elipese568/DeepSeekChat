using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Core;

public abstract class Readonly<T>
{
    protected readonly T _value;
    public Readonly(T value)
    {
        _value = value;
    }

    public abstract object GetValue(string memberName);
}

public class ReadonlyProperty<T, TOwner>
{
    private ReadonlyProperty(string name) { Name = name; }

    public string Name { get; }
    
    public T GetValue(Readonly<TOwner> @readonly)
    {
        return (T)@readonly.GetValue(Name);
    }

    public static ReadonlyProperty<T, TOwner> Create(string name)
    {
        return new ReadonlyProperty<T, TOwner>(name);
    }
}

public interface IReadonlyConvertable<T>
{
    public Readonly<T> AsReadonly();
}