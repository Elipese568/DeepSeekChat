using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using DeepSeekChat.Helper.Converters;
using DeepSeekChat.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DeepSeekChat
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            EmptyVisibilityConverter.RegisterHandler(typeof(string), (value) => !string.IsNullOrEmpty((string)value));
            EmptyVisibilityConverter.RegisterHandler(typeof(int), (value) => (int)value != 0);
            EmptyVisibilityConverter.RegisterHandler(typeof(double), (value) => (double)value > 0);

            EmptyVisibilityConverter.RegisterHandler(typeof(List<>), (value) => (value as ICollection).Count != 0);
            EmptyVisibilityConverter.RegisterHandler(typeof(ObservableCollection<>), (value) => (value as ICollection).Count != 0);
            EmptyVisibilityConverter.RegisterHandler(typeof(ObservableCollection<DiscussItem>), (value) => (value as ICollection).Count != 0);
            EmptyVisibilityConverter.RegisterHandler(typeof(ItemCollection), (value) => (value as ItemCollection).Count != 0);
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
        }

        private Window? m_window;
    }
}
