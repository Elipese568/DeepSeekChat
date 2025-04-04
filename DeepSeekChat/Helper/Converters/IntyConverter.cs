using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Helper.Converters;

public class IntyConverter : IValueConverter
{
    private object ConvertInternal(object value, Type targetType, string parameter)
    {
        int round = int.Parse(parameter ?? "2");

        if(targetType == typeof(double))
        {
            double input = System.Convert.ToDouble(value);
            return Math.Round(input, round);
        }
        else if (targetType == typeof(float))
        {
            float input = System.Convert.ToSingle(value);
            return MathF.Round(input, round);
        }

        return null;
    }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if(value.ToString().Length == 1 && value.ToString() != "0")
            return global::System.Convert.ChangeType(value, targetType);
        if (value.ToString().StartsWith("0.1")) // solve 0.10000015
            return global::System.Convert.ChangeType("0.1", targetType);
        return ConvertInternal(value, targetType, (string)parameter) ?? DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value.ToString().Length == 1 && value.ToString() != "0")
            return global::System.Convert.ChangeType(value, targetType);
     /* if (value.ToString().StartsWith("0.1")) 
      *      return global::System.Convert.ChangeType("0.1", targetType);
      * Solving 0.10000015, but it cannot solve this problem and even stills 0.10000015 in ConvertBack method
      * so I disabled 
      * WTF????????? */

        return ConvertInternal(value, targetType, (string)parameter) ?? DependencyProperty.UnsetValue;
    }
}
