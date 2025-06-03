using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RecallShield.Services;
using RecallShield.UI;

namespace RecallShield;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        var app = new App();
        app.InitializeComponent();
        app.Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<RecallBlockerService>();
                services.AddSingleton<ScreenshotBlockerService>();
                services.AddSingleton<SystemTrayService>();
                services.AddSingleton<MainWindow>();
            });
} 