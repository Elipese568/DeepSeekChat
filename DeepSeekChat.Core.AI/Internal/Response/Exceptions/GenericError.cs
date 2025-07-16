using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Core.AI.Internal.Response.Exceptions;

internal class GenericError
{
    public int Code { get; set; }
    public string Message { get; set; }
    public string Data { get; set; }
}
