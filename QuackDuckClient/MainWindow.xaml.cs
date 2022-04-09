using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using VkNet;
using VkNet.Abstractions.Authorization;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.NLog.Extensions.Logging;
using VkNet.NLog.Extensions.Logging.Extensions;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using QuackDuckClient.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;

namespace QuackDuckClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
	{
		private VkApi _api;

		public MainWindow()
		{
			InitializeComponent();
			DataContext = new ChatTapeViewModel(_api);
		}

        /// <summary>
        /// Calls on this element is initialized. Authorizes user.
        /// </summary>
        private void Window_Initialized(object sender, System.EventArgs e)
        {
            _api = new VkApi(InitDi());

            if (_api.IsAuthorized)
            {
                return;
            }

			_api.Authorize(new ApiAuthParams
			{
				ApplicationId = GetAppId(),
				Settings = Settings.All | Settings.Messages
			});
            _ = new LongPollingManager(_api);
        }

		private static ulong GetAppId()
		{
			// асинхронное чтение AppID из файла
			using (StreamReader reader = new StreamReader("appid.txt"))
			{
				string text = reader.ReadToEnd();
				return Convert.ToUInt64(text);
			}
		}

		/// <summary>
		/// Dependency Injection services generation
		/// </summary>
		/// <returns>Service collection</returns>
		private static ServiceCollection InitDi()
		{
			var di = new ServiceCollection();

			di.AddSingleton<IAuthorizationFlow, WpfAuthorize>();
			di.AddSingleton<ILoggerFactory, LoggerFactory>();
			di.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
			di.AddLogging(builder =>
			{
				builder.ClearProviders();
				builder.SetMinimumLevel(LogLevel.Trace);
				builder.AddNLog(new NLogProviderOptions
				{
					CaptureMessageProperties = true,
					CaptureMessageTemplates = true
				});
			});
			LogManager.LoadConfiguration("nlog.config");

			return di;
		}
    }
}
