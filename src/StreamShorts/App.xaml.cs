using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Extensions.DependencyInjection;

using StreamShorts.Library;
using StreamShorts.MVVM;
using StreamShorts.PC;
using StreamShorts.Projects;
using StreamShorts.Projects.Processing;
using StreamShorts.ViewModels;
using StreamShorts.Views;
using StreamShorts.YouTube;

namespace StreamShorts
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private IServiceProvider? _serviceProvider;
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			ConfigureServices();
      _serviceProvider!.GetRequiredService<SettingsRepo>().LoadSettings();
      MainWindow = _serviceProvider!.GetService<MainWindow>();
			MainWindow!.Show();
		}
		public void ConfigureServices()
		{
			ServiceCollection services = new ServiceCollection();

			services.AddSingleton<MainWindow>();
			services.AddSingleton<MainWindowViewModel>();
      services.AddKeyedTransient<IPage, AddProject>("AddProject");
      services.AddTransient<AddProjectViewModel>();
      services.AddKeyedSingleton<IPage, ExistingProjects>("ExistingProjects");
      services.AddSingleton<ExistingProjectsViewModel>();
      services.AddKeyedSingleton<IPage, InstallFFMpeg>("InstallFFMpeg");
      services.AddSingleton<InstallFFMpegViewModel>();
      services.AddKeyedSingleton<IPage, InstallOllama>("InstallOllama");
      services.AddSingleton<InstallOllamaViewModel>();
      services.AddKeyedSingleton<IPage, ProjectDetails>("ProjectDetails");
      services.AddSingleton<ProjectDetailsViewModel>();
      services.AddKeyedSingleton<IPage, Views.Settings>("Settings");
      services.AddSingleton<SettingsViewModel>();

      services.AddSingleton<IFileSystem, FileSystem>();
      services.AddStreamShorts();
      services.AddTransient<YouTubeDownload>();
      services.AddSingleton<KeepPCAwake>();
      services.AddSingleton<ProjectFolderRepo>();
      services.AddSingleton<Project>();
      services.AddSingleton<VideoQueueManager>();
      services.AddTransient<VideoWorkItem>();
      services.AddTransient<VideoWorkItemProcessor>();
      services.AddTransient(typeof(IFactory<>), typeof(ServiceProviderFactory<>));

      _serviceProvider = services.BuildServiceProvider();
		}
	}
}
