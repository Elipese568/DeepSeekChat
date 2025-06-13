using DeepSeekChat.Foundation;
using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    private EventHandlerWrapper<EventHandler<SettingChangedEventArgs>> _settingChangedHandlers;

    public SettingService()
    {
        _innerApplicationDataInstance = ApplicationData.GetDefault();
        _settingChangedHandlers = EventHandlerWrapper<EventHandler<SettingChangedEventArgs>>.Create();
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

        _settingChangedHandlers.Invoke(this, new SettingChangedEventArgs
        {
            Name = key,
            Value = value
        });
    }

    public event EventHandler<SettingChangedEventArgs> SettingChanged
    {
        add
        {
            _settingChangedHandlers.AddHandler(value);
        }
        remove
        {
            _settingChangedHandlers.RemoveHandler(value);
        }
    }

    public const string SETTING_THEME = "ApplicationTheme";
    public const string SETTING_APIKEY = "ApiKey";
    public const string SETTING_SELECTED_MODEL = "SelectedModel";
    public const string SETTING_DISPLAY_LANGUAGE = "DisplayLanguage";
    public const string SETTING_IS_USE_DISPLAY_LANGUAGE_ANSWER = "IsUseDisplayLanguageAnswer";
}
