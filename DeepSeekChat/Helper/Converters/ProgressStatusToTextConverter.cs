using DeepSeekChat.Models;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Helper.Converters;

public partial class ProgressStatusToTextConverter : IValueConverter
{
    public readonly static Dictionary<ProgressStatus, Func<string>> ProgressStatusToTextTable = new()
    {
        [ProgressStatus.InProgress] = "InProgressText".GetLocalized,
        [ProgressStatus.Completed] = "CompletedText".GetLocalized,
        [ProgressStatus.Stoped] = "StopedText".GetLocalized,
        [ProgressStatus.LengthTerminated] = "LengthTerminatedText".GetLocalized,
        [ProgressStatus.Failed] = "FailedText".GetLocalized,
    };
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return ProgressStatusToTextTable[(ProgressStatus)value]();
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
