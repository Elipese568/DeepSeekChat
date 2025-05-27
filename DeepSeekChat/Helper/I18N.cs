using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Markup;

namespace DeepSeekChat.Helper;

[MarkupExtensionReturnType(ReturnType = typeof(string))]
public class I18N : MarkupExtension
{
    public string Key { get; set; } = string.Empty;
    public string Map { get; set; } = string.Empty;

    protected override object ProvideValue()
    {
        return Key.GetLocalized(Map);  
    }
}
