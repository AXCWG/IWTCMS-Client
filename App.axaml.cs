using System;
using System.IO;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace ws
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
                desktop.Exit += (s, e) =>
                {
                    File.WriteAllText("./config.json", JsonSerializer.Serialize(Singletons.configs, new JsonSerializerOptions() { IncludeFields = true }));
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}