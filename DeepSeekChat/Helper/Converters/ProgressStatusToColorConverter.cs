using DeepSeekChat.Models;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Helper.Converters;
public partial class ProgressStatusToColorConverter : IValueConverter
{
    public readonly static Dictionary<ProgressStatus, SolidColorBrush> ProgressStatusToColorTable = new()
    {
        [ProgressStatus.None] = new SolidColorBrush(Windows.UI.Color.FromArgb(0,0,0,0)),
        [ProgressStatus.InProgress] = (SolidColorBrush)App.Current.Resources["SystemFillColorSuccessBrush"],
        [ProgressStatus.Completed] = (SolidColorBrush)App.Current.Resources["SystemFillColorAttentionBrush"],
        [ProgressStatus.Stoped] = (SolidColorBrush)App.Current.Resources["SystemFillColorNeutralBrush"],
        [ProgressStatus.LengthTerminated] = (SolidColorBrush)App.Current.Resources["SystemFillColorCautionBrush"],
        [ProgressStatus.Failed] = (SolidColorBrush)App.Current.Resources["SystemFillColorCriticalBrush"],
    };
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return ProgressStatusToColorTable[(ProgressStatus)value];
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
