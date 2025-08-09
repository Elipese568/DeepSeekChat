using DeepSeekChat.Views;
using Microsoft.UI.Xaml;
using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Helper;
public static class ResourceExtension
{
    public static void Initialize() { }
    public static string GetLocalized(this string resourceKey, string map) 
    {
        return new ResourceLoader(ResourceLoader.GetDefaultResourceFilePath(), map).GetString(resourceKey.Replace(".","/"));
    }

    public static string GetLocalized(this string resourceKey)
    {
        return new ResourceLoader(ResourceLoader.GetDefaultResourceFilePath(), "General").GetString(resourceKey.Replace(".", "/"));
    }

    public static T GetResource<T>(this string resourceKey, FrameworkElement? element = null, ResourceDictionary? rd = null)
    {
        if (element == null)
            if (rd == null)
                return (T)MainPage.Current.Resources[resourceKey];
            else
                return (T)rd[resourceKey];
        else
            return (T)element.Resources[resourceKey];
    }

    //public static ResourceCandidate GetLocalizedRaw(this string resourceKey, string map = "Resources") => _resources[map + "/" + resourceKey];
}
