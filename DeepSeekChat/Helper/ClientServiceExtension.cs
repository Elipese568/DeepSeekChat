using DeepSeekChat.Models;
using DeepSeekChat.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Helper;

public static class ClientServiceExtension
{
    public static ClientService ConfigureClientServiceFromSetting(this ClientService clientService)
    {
        var settingService = App.Current.GetService<SettingService>();
        if (settingService == null)
            return clientService;
        clientService.ConfigureAll(
            new("https://api.siliconflow.cn/v1/"), // TODO: Replace with a setting after this option is unlocked
            settingService.Read(SettingService.SETTING_APIKEY, string.Empty),
            App.Current.GetService<ModelsManagerService>().GetModelById(new Guid(settingService.Read(SettingService.SETTING_SELECTED_MODEL, AiModelStorage.DEEPSEEK_DEFAULT_MODEL_GUID))).ModelID);
        return clientService;
    }
}
