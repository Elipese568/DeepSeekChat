using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Helper;

public static class WindowHelper
{
    public static Task WaitForWindowCloseAsync(Window window)
    {
        var tcs = new TaskCompletionSource<object?>();

        // 监听窗口关闭事件
        window.Closed += (sender, args) =>
        {
            tcs.TrySetResult(null);
        };

        window.Activate(); // 激活窗口
        return tcs.Task;
    }
}
