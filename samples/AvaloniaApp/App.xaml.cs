using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Logging.Serilog;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.ThemeManager;

namespace AvaloniaApp
{
    public class App : Application
    {
        public static IThemeSelector? Selector { get; private set; }

        [STAThread]
        static void Main(string[] args)
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .UseReactiveUI()
                         .LogToDebug();

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                Selector = ThemeSelector.LoadSafe("Themes");
                desktopLifetime.MainWindow = new MainWindow()
                {
                    DataContext = Selector
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
