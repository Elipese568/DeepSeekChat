using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Service;

public sealed class SettingChangedEventArgs : EventArgs
{
    public string Name { get; set; }
    public string Value { get; set; }
}
class SettingService
{
    private ApplicationData _innerApplicationDataInstance;
    private List<EventHandler<SettingChangedEventArgs>> _settingChangedHandlers = new();

    public SettingService()
    {
        _innerApplicationDataInstance = ApplicationData.GetDefault();
    }
    public string Read(string key, string? defaultValue = default)
    {
        if (_innerApplicationDataInstance.LocalSettings.Values.TryGetValue(key, out object result))
        {
            return (string)result;
        }
        return defaultValue;
    }

    public void Write(string key, string value)
    {
        if (!_innerApplicationDataInstance.LocalSettings.Values.TryAdd(key, value))
            _innerApplicationDataInstance.LocalSettings.Values[key] = value;

        foreach(var handler in _settingChangedHandlers)
        {
            handler.Invoke(this, new() { Name = key, Value = value });
        }
    }

    public event EventHandler<SettingChangedEventArgs> SettingChanged
    {
        add
        {
            if (!_settingChangedHandlers.Contains(value))
                _settingChangedHandlers.Add(value);
        }
        remove
        {
            if (_settingChangedHandlers.Contains(value))
                _settingChangedHandlers.Remove(value);
        }
    }

    public const string SETTING_THEME = "ApplicationTheme";
    public const string SETTING_APIKEY = "ApiKey";
    public const string SETTING_SELECTED_MODEL = "SelectedModel";
}
