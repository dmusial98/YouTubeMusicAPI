using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YouTubeMusicAPI.Services;
using YouTubeMusicAPI.Services.Interfaces;
using YouTubeMusicAPI.WorkPlan;

namespace YouTubeMusicAPI
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			using var host = CreateHostBuilder(args).Build();
			await host.Services.GetRequiredService<App>().Run();

			Console.ReadLine();
			Console.ReadLine();
		}

		static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureServices((_, services) =>
				{
					services.AddTransient<IMusicDownloader, MusicDownloader>();
					services.AddTransient<ISettingsReader, SettingsReader>();
					services.AddTransient<ISettingsValidator, SettingsValidator>();
					services.AddTransient<IUrlFileReaderWriter, UrlFileReaderWriter>();
					services.AddSingleton<IYTApiCommunicator, YTApiCommunicator>();
					services.AddTransient<IWorkDispatcher, WorkDispatcher>();
					services.AddTransient<IFileChecker, FileChecker>();
					services.AddTransient<IFilesRenamer, FilesRenamer>();
					services.AddTransient<App>();
				});
	}
}
