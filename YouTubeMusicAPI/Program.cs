using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using TagLib.Id3v2;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using YouTubeMusicAPI.Services;
using YouTubeMusicAPI.Services.Interfaces;
using YouTubeMusicAPI.SettingsStructure;
using YouTubeMusicAPI.WorkPlan;

namespace YouTubeMusicAPI
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			// Tworzymy hosta dla aplikacji konsolowej
			using var host = CreateHostBuilder(args).Build();

			// Uruchamiamy główną logikę aplikacji z wstrzykniętymi zależnościami
			await host.Services.GetRequiredService<App>().Run();
		}

		static IHostBuilder CreateHostBuilder(string[] args) =>
			 Host.CreateDefaultBuilder(args)
					 .ConfigureServices((_, services) =>
					 {
						 services.AddTransient<IFFmpegConnector, FFmpegConnector>();
						 services.AddTransient<IMusicDownloader, MusicDownloader>();
						 services.AddTransient<ISettingsReader, SettingsReader>();
						 services.AddTransient<ISettingsValidator, SettingsValidator>();
						 services.AddTransient<ITagger, Tagger>();
						 services.AddTransient<IUrlFileReaderWriter, UrlFileReaderWriter>();
						 services.AddSingleton<IYTApiCommunicator, YTApiCommunicator>();
						 services.AddTransient<IWorkDispatcher, WorkDispatcher>();

						 services.AddTransient<App>(); // Rejestrujemy klasę App, która korzysta z wstrzykniętych zależności
					 });
	}
}
