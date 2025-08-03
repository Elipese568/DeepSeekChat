using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using DeepSeekChat.Foundation;
using DeepSeekChat.Helper;
using DeepSeekChat.Helper.Converters;
using DeepSeekChat.Models;
using DeepSeekChat.Service;
using DeepSeekChat.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Notifications;

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
            EmptyVisibilityConverter.RegisterHandler(typeof(ObservableCollection<ContentPartViewModel>), (value) => (value as ObservableCollection<ContentPartViewModel>).Count != 0);
            EmptyVisibilityConverter.RegisterHandler(typeof(ObservableCollection<DiscussionItem>), (value) => (value as ObservableCollection<DiscussionItem>).Count != 0);
            EmptyVisibilityConverter.RegisterHandler(typeof(ObservableCollection<FileViewModel>), (value) => (value as ObservableCollection<FileViewModel>).Count != 0);
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
                .AddSingleton<FileManagerService>()
                .AddSingleton<OcrService>()
                .BuildServiceProvider()
            );

            Current = this;
            m_exitProcess = EventHandlerWrapper<EventHandler>.Create();

            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                m_exitProcess.Invoke(this, EventArgs.Empty);
                m_exceptionPipe?.Close();
            };

            UnhandledException += UnhandledExceptionProcessor;

            DefaultJsonTypeInfoResolver exceptionJsonTypeResolver = new DefaultJsonTypeInfoResolver();
            exceptionJsonTypeResolver.Modifiers.Add(x =>
            {
                int tsPropIdx = x.Properties.IndexOf(x => x.Name == "TargetSite");
                if (tsPropIdx != -1)
                    x.Properties.RemoveAt(tsPropIdx);
            });
            m_exceptionJsonSerializeOptions = new()
            {
                TypeInfoResolver = exceptionJsonTypeResolver
            };
        }

        private const string ExceptionDataPipeName = "DeepSeekChatExceptionPipe";
        private JsonSerializerOptions m_exceptionJsonSerializeOptions;

        private record struct PipeData(string Type, Dictionary<string, string> Arguments);

        private void UnhandledExceptionProcessor(object s, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            var notification = new AppNotificationBuilder()
                .AddText("UnexpectedExceptionText".GetLocalized())
                .AddText(e.Exception.Message)
                .AddButton(new AppNotificationButton("SeeDetailText".GetLocalized()))
                .BuildNotification();

            var exceptionPipeData = new PipeData("exception", new Dictionary<string, string>()
            {
                ["exception_serialized_string"] = JsonSerializer.Serialize(e.Exception, m_exceptionJsonSerializeOptions),
                ["exception_type"] = e.Exception.GetType().FullName
            });

            notification.Priority = Microsoft.Windows.AppNotifications.AppNotificationPriority.High;
            AppNotificationManager.Default.Show(notification);
            e.Handled = true;

            ExceptionPipeHolder(exceptionPipeData);
        }

        private NamedPipeServerStream m_exceptionPipe;
        private byte[] m_receivedFlag = [114,51,4,191,98,10];
        private CancellationTokenSource m_waitReceiveCts;
        private CancellationTokenSource m_waitConnectCts;
        private async Task ExceptionPipeHolder(PipeData data)
        {
            m_waitReceiveCts?.Cancel();
            m_waitConnectCts?.Cancel();
            m_exceptionPipe ??= new NamedPipeServerStream(ExceptionDataPipeName);
            
            m_waitConnectCts = new CancellationTokenSource();
            m_waitConnectCts.CancelAfter(TimeSpan.FromMinutes(10));
            await m_exceptionPipe.WaitForConnectionAsync(m_waitConnectCts.Token);
            
            byte[] serializedData = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
            m_exceptionPipe.Write(BitConverter.GetBytes(serializedData.Length));
            m_exceptionPipe.Write(serializedData);

            m_waitReceiveCts = new CancellationTokenSource();
            byte[] receivedBytes = new byte[6];
            await m_exceptionPipe.ReadAsync(receivedBytes, m_waitReceiveCts.Token);
            if(m_waitReceiveCts.IsCancellationRequested)
                return;

            m_exceptionPipe.Close();
            m_exceptionPipe = null;
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            NamedPipeClientStream clientStream = new NamedPipeClientStream(ExceptionDataPipeName);
            try
            {
                clientStream.Connect(50);
                Span<byte> lengthSpan = stackalloc byte[4];
                clientStream.Read(lengthSpan);
                int length = BitConverter.ToInt32(lengthSpan);
                byte[] data = new byte[length];
                clientStream.Read(data, 0, length);
                clientStream.Write(m_receivedFlag);
                clientStream.Dispose();

                PipeData pipeData = JsonSerializer.Deserialize<PipeData>(data);
                switch(pipeData.Type)
                {
                    case "exception":
                        ProcessExceptionNotification(pipeData);
                        return;
                }
            }
            catch(TimeoutException)
            {
                clientStream.Dispose();
                Debug.WriteLine(GetService<SettingService>().Read(SettingService.SETTING_DISPLAY_LANGUAGE, ""));
                Microsoft.Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = GetService<SettingService>().Read(SettingService.SETTING_DISPLAY_LANGUAGE, "zh-Hans-CN");
                Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = GetService<SettingService>().Read(SettingService.SETTING_DISPLAY_LANGUAGE, "zh-Hans-CN");
                m_window = new MainWindow();
                m_window.Activate();
            }
        }

        private void ProcessExceptionNotification(PipeData pipeData)
        {
            JsonDocument jsonDocument = JsonDocument.Parse(pipeData.Arguments["exception_serialized_string"]);

            StringBuilder exceptionDescriptionBuilder = new();
            exceptionDescriptionBuilder.Append("Exception Type:");
            exceptionDescriptionBuilder.AppendLine(pipeData.Arguments["exception_type"]);

            void ProcessJsonObject(JsonElement element, string indent = "")
            {
                if (element.ValueKind == JsonValueKind.Object)
                {
                    foreach (var property in element.EnumerateObject())
                    {
                        exceptionDescriptionBuilder.Append(indent);
                        exceptionDescriptionBuilder.Append(property.Name);
                        exceptionDescriptionBuilder.Append(": ");
                        ProcessJsonObject(property.Value, indent + "  ");
                    }
                }
                else if (element.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in element.EnumerateArray())
                    {
                        ProcessJsonObject(item, indent + "  ");
                    }
                }
                else
                {
                    exceptionDescriptionBuilder.AppendLine(element.ToString());
                }
            }

            ProcessJsonObject(jsonDocument.RootElement);

            m_window = new ExceptionPreviewWindow();
            ((ExceptionPreviewWindow)m_window).ExceptionString = exceptionDescriptionBuilder.ToString();
            m_window.Activate();
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
