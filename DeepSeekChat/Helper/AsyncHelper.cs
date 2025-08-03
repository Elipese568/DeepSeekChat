using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace DeepSeekChat.Helper;

public static class AsyncHelper
{
    public static T Sync<T>(this IAsyncOperation<T> windowsAsyncOperation)
    {
        return windowsAsyncOperation.GetAwaiter().GetResult();
    }

    public static void Sync(this IAsyncAction windowsAsyncAction)
    {
        windowsAsyncAction.GetAwaiter().GetResult();
    }

    public static T Sync<T>(this Task<T> asyncOperation)
    {
        return asyncOperation.GetAwaiter().GetResult();
    }
}
