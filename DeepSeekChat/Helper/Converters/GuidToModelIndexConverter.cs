using DeepSeekChat.Service;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Helper.Converters;

public class GuidToModelIndexConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return App.Current.GetService<ModelsManagerService>()
            .GetStroragedModels()
            .IndexOf(x => x.UniqueID == (Guid)value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return App.Current.GetService<ModelsManagerService>()
            .GetStroragedModels()[(int)value > 0? (int)value : 0].UniqueID;
    }
}
