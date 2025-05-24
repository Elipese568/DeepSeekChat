using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CommunityToolkit.Mvvm.DependencyInjection;
using DeepSeekChat.Foundation;
using DeepSeekChat.Helper;
using DeepSeekChat.Helper.Converters;
using DeepSeekChat.Models;
using DeepSeekChat.Service;
using Microsoft.Extensions.DependencyInjection;
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
using Windows.UI;

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
            EmptyVisibilityConverter.RegisterHandler(typeof(ObservableCollection<DiscussionItem>), (value) => (value as ICollection).Count != 0);
            EmptyVisibilityConverter.RegisterHandler(typeof(ItemCollection), (value) => (value as ItemCollection).Count != 0);

            EmptyVisibilityConverter.RegisterHandler(typeof(SolidColorBrush), v => ((SolidColorBrush)v).Color.A > 0);

            DictionaryConeverter.RegisterHandler((typeof(ProgressStatus), typeof(int)), (x, y) =>
            {
                return (int)x == (int)y;
            });
            DictionaryConeverter.RegisterHandler((typeof(ProgressStatus), typeof(ProgressStatus)), (x, y) =>
            {
                return (int)x == (int)y;
            });

            m_ioc = new();
            m_ioc.ConfigureServices(
                new ServiceCollection()
                .AddSingleton<DiscussionItemService>()
                .AddSingleton<SettingService>()
                .AddSingleton<ModelsManagerService>()
                .AddSingleton<ClientService>()
                .AddSingleton<AvatarManagerService>()
                .BuildServiceProvider()
            );

            Current = this;
            m_exitProcess = EventHandlerWrapper<EventHandler>.Create();

            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                m_exitProcess.Invoke(this, EventArgs.Empty);
            };
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
            ResourceExtension.Initialize();
        }

        public T? GetService<T>()
        {
            return m_ioc.GetService<T>();
        }

        private Window? m_window;
        private readonly Ioc m_ioc;

        private EventHandlerWrapper<EventHandler> m_exitProcess;
        public event EventHandler ExitProcess
        {
            add
            {
                m_exitProcess.AddHandler(value);
            }
            remove
            {
                m_exitProcess.RemoveHandler(value);
            }
        }

        public static new App Current { get; private set; }
    }
}
