using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Helper;

public static class SettingHelper
{
    private static ApplicationData _innerApplicationDataInstance;

    static SettingHelper()
    {
        _innerApplicationDataInstance = ApplicationData.GetDefault();
    }
    public static T Read<T>(string key, T? defaultValue = default)
    {
        if (_innerApplicationDataInstance.LocalSettings.Values.TryGetValue(key, out object result) )
        {
            return (T)result;
        }
        return defaultValue;
    }

    public static void Write<T>(string key, T value)
    {
        if (_innerApplicationDataInstance.LocalSettings.Values.TryAdd(key, value))
            _innerApplicationDataInstance.LocalSettings.Values[key] = value;
    }
}
