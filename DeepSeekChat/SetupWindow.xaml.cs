using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DeepSeekChat;

public class ModelItem
{
    public string Name { get; set; }  // 左侧标题
    public string Tag { get; set; }   // 右侧描述
}

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SetupWindow : Window
{
    public SetupWindow()
    {
        ExtendsContentIntoTitleBar = true;
        AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Standard;
        AppWindow.TitleBar.ButtonBackgroundColor = Microsoft.UI.Colors.Transparent;
        AppWindow.Resize(new Windows.Graphics.SizeInt32(340, 500));
        InitializeComponent();
        SetTitleBar(MyTitleBar);
        // 原 DSC StackPanel 中的内容
        DscItems = new ObservableCollection<ModelItem>
        {
            new ModelItem { Name = "DeepSeekV3.1-Chat", Tag = "deepseek-chat" },
            new ModelItem { Name = "DeepSeekV3.1-Reasoner", Tag = "deepseek-reasoner" }
        };

        // 原 SF StackPanel 中的内容
        SfItems = new ObservableCollection<ModelItem>
        {
            new ModelItem { Name = "DeepSeek-V3.1", Tag = "deepseek-ai/DeepSeek-V3.1" },
            new ModelItem { Name = "DeepSeek-V3.1(Pro)", Tag = "Pro/deepseek-ai/DeepSeek-V3.1" },
            new ModelItem { Name = "DeepSeek-R1", Tag = "deepseek-ai/DeepSeek-R1" },
            new ModelItem { Name = "DeepSeek-R1(Pro)", Tag = "Pro/deepseek-ai/DeepSeek-R1" },
            new ModelItem { Name = "DeepSeek-V3", Tag = "deepseek-ai/DeepSeek-V3" },
            new ModelItem { Name = "DeepSeek-V3(Pro)", Tag = "Pro/deepseek-ai/DeepSeek-V3" }
        };
    }

    public ObservableCollection<ModelItem> DscItems { get; set; }
    public ObservableCollection<ModelItem> SfItems { get; set; }

    private void StepSelector_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        if((string)sender.SelectedItem.Tag == "Models")
        {

        }
    }
}
