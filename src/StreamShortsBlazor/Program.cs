using System.IO.Abstractions;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using StreamShorts.Library;
using StreamShorts.MVVM.Install;
using StreamShorts.MVVM.Interfaces;
using StreamShorts.MVVM.MVVM;
using StreamShorts.MVVM.Projects;
using StreamShorts.MVVM.Projects.Processing;
using StreamShorts.MVVM.ViewModels;
using StreamShorts.MVVM.YouTube;

using StreamShortsBlazor.Pages;
using StreamShortsBlazor.UI;

using StreamShortsMVVM.Interfaces;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddStreamShorts();
builder.Services.AddScoped<INavigationEvent, NavigationEvent>();
builder.Services.AddScoped<AddProjectViewModel>();
builder.Services.AddScoped<ExistingProjectsViewModel>();
builder.Services.AddScoped<ProjectDetailsViewModel>();
builder.Services.AddScoped<SettingsViewModel>();
builder.Services.AddScoped<IFileSystem, FileSystem>();
builder.Services.AddTransient<YouTubeDownload>();
builder.Services.AddScoped<ProjectFolderRepo>();
builder.Services.AddTransient<Project>();
builder.Services.AddScoped<VideoQueueManager>();
builder.Services.AddTransient<VideoWorkItem>();
builder.Services.AddTransient<VideoWorkItemProcessor>();
builder.Services.AddTransient(typeof(IFactory<>), typeof(ServiceProviderFactory<>));
builder.Services.AddTransient<OllamaVerification>();
builder.Services.AddTransient<ISettingsCanSave, SettingsCanSave>();
builder.Services.AddTransient<IUiDispatcher, UiDispatcher>();
builder.Services.AddHttpClient();
builder.Services.AddTransient<IMessageBox, MessageBoxWrapper>();
builder.Services.AddScoped<IEnvironment, StreamShortsBlazor.UI.Environments>();
var app = builder.Build();

app.Services.GetService<SettingsRepo>()?.LoadSettings();

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
