using DeepSeekChat.Core.Network.Body;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Core.Network.Request;

public interface IRequestBodyConvertable
{
    public RequestBody ToRequestBody();
}

public interface IRequestBodyConvertable<TRequestBody> : IRequestBodyConvertable
    where TRequestBody : RequestBody
{
    public TRequestBody ToRequestBody();
    RequestBody IRequestBodyConvertable.ToRequestBody() => ToRequestBody();
}

public interface IRequestBodyCollectionConvertable<T>
    where T : RequestBody
{
    public T[] ToRequestBodys();
}