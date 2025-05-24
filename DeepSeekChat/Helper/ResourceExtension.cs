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
    public static string GetLocalized(this string resourceKey, string culture = "General") 
    {
        return new ResourceLoader(ResourceLoader.GetDefaultResourceFilePath(), culture).GetString(resourceKey.Replace(".","/"));
    }

    //public static ResourceCandidate GetLocalizedRaw(this string resourceKey, string culture = "Resources") => _resources[culture + "/" + resourceKey];
}
