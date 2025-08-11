using Microsoft.Extensions.DependencyInjection;
using StreamShorts.ViewModels;
using StreamShorts.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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
			MainWindow = _serviceProvider!.GetService<MainWindow>();
			MainWindow!.Show();
		}
		public void ConfigureServices()
		{
			ServiceCollection services = new ServiceCollection();
			services.AddSingleton<MainWindow>();
			services.AddSingleton<MainWindowViewModel>();

			_serviceProvider = services.BuildServiceProvider();
		}
	}
}
