using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Helper;

public static class ControlExtension
{
    public static T LinkSet<T>(this T control, Action<T> setAction)
        where T : FrameworkElement
    {
        setAction(control);
        return control;
    }
}
