using DeepSeekChat.Helper;
using DeepSeekChat.Service;
using DeepSeekChat.ViewModels;
using DeepSeekChat.Views;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.ApplicationModel.Core;
using Windows.UI.Core.Preview;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DeepSeekChat
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public static new MainWindow Current { get; private set; }

        public MainWindow() : base()
        {
            ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Tall;
            AppWindow.TitleBar.ButtonBackgroundColor = Microsoft.UI.Colors.Transparent;
            Current = this;

            AccessKeyManager.AreKeyTipsEnabled = false;

            SetTitleBar(MyTitleBar);
            this.InitializeComponent();
        }

        private bool _inQuickMode = false;

        public void DrillInQuickDiscussionMode(DiscussionItemViewModel ndVM)
        {
            _inQuickMode = true;
            ContentFrame.Navigate(typeof(DiscussionPage), ndVM, new DrillInNavigationTransitionInfo());
        }

        private async void MyTitleBar_BackRequested(TitleBar sender, object args)
        {
            if(_inQuickMode)
            {
                bool flowControl = await ExitQuickMode();
                if (!flowControl)
                {
                    return;
                }

                AppWindow.SetPresenter(AppWindowPresenterKind.Default);
                ContentFrame.GoBack();
                _inQuickMode = false;
            }
            else
                if (MainPage.Current.Frame.BackStackDepth > 0)
                    MainPage.Current.Frame.GoBack();
        }

        private async System.Threading.Tasks.Task<bool> ExitQuickMode()
        {
            var result = await ContentDialogHelper.ShowMessageDialog(
                                "NeedConfirmOperationText".GetLocalized(),
                                "BackFromQuickModeDialog.Content".GetLocalized("MainWindow"),
                                "ConfirmText".GetLocalized(),
                                "CancelText".GetLocalized(),
                                "BackFromQuickModeDialog.NoNeedContent".GetLocalized("MainWindow"),
                                ContentDialogButton.Primary,
                                Content.XamlRoot);

            if (result == ContentDialogResult.None)
                return false;
            else if (result == ContentDialogResult.Primary)
            {
                TextBox textBox = new()
                {
                    Header = "InputTitleTextBox.Header".GetLocalized("MainPage"),
                    PlaceholderText = "InputTitleTextBox.PlaceholderText".GetLocalized("MainPage"),
                    MaxLength = 32
                };

                await ContentDialogHelper.ShowContentDialog(
                    "AddDiscussionItemText".GetLocalized("MainWindow"),
                    textBox,
                    primaryButtonText: "AddText".GetLocalized(),
                    "CancelText".GetLocalized(),
                    null,
                    ContentDialogButton.Primary,
                    Content.XamlRoot,
                    async (s, e) =>
                    {
                        if (textBox.Text.Length == 0)
                        {
                            e.Cancel = true;
                        }
                        else
                        {
                            var ndVM = (ContentFrame.Content as DiscussionPage).ViewModel;
                            ndVM.SelectedDiscussItemViewModel.Title = textBox.Text;
                            App.Current.GetService<DiscussionItemService>().InjectDiscussionItem(ndVM.SelectedDiscussItemViewModel.InnerObject);
                        }
                    });
            }

            return true;
        }
    }
}
