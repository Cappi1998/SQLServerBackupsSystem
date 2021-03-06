using Coravel;
using Serilog;
using SQLServerBackupsSystem.Interfaces;
using SQLServerBackupsSystem.Schedules;
using SQLServerBackupsSystem.Models;

class Program
{
    public static string Base_Path = Directory.GetCurrentDirectory();
    public static SQLServerConfig sqlConfig = new SQLServerConfig();
    public static List<int> HoursOfDayToRunBackup = new List<int>();
    public static int MaxDaysToStorageBackup = 90;
    public static string DiscordWebHook_NotificationsURL = "";
    public static void Main(string[] args)
    {
        Console.Title = "SQLServerBackupsSystem";
        
        Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(Directory.GetCurrentDirectory(), "log-.txt"), rollingInterval: RollingInterval.Day)
                .CreateLogger();

        LoadConfig();

        try
        {
            Log.Information("Starting Aplication");
            IHost host = CreateHostBuilder(args).Build();
            host.Services.UseScheduler(scheduler =>
            {
                scheduler.Schedule<BucketStatusCheck>().DailyAtHour(2).PreventOverlapping($"BucketStatusCheck");

                if (HoursOfDayToRunBackup == null || HoursOfDayToRunBackup.Count == 0)
                {
                    Log.Information($"No backup schedule has been set, backup has been set to run once a day!");
                    scheduler.Schedule<AutomaticProcess>().Daily().PreventOverlapping($"AutomaticProcess");
                }
                else
                {
                    foreach (int hr in HoursOfDayToRunBackup)
                    {
                        Log.Information($"Schedule to run backup every day at {hr} o'clock created");
                        scheduler.Schedule<AutomaticProcess>().DailyAtHour(hr).PreventOverlapping($"AutomaticProcess_{hr}");
                    }
                }

            });
            host.Run();

        }
        catch (Exception ex)
        {
            Console.WriteLine("Application start-up failed", ex.InnerException);
            Console.Read();
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {

        var host = Host.CreateDefaultBuilder(args).UseSerilog();

        host.ConfigureServices((hostContext, services) => {
                    services.AddSingleton<ICloudStorage, GoogleCloudStorage>();
                    services.AddScheduler();

                    //Coravel Service
                    services.AddTransient<AutomaticProcess>(); 
                    services.AddTransient<BucketStatusCheck>();

        });

        return host;
    }

    public static void LoadConfig()
    {
        var Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        Program.DiscordWebHook_NotificationsURL = Configuration.GetValue<string>("DiscordWebHook_NotificationsURL");
        Log.Information($"DiscordWebHook_NotificationsURL: {Program.DiscordWebHook_NotificationsURL}");

        Program.MaxDaysToStorageBackup = Configuration.GetValue<int>("MaxDaysToStorageBackup");
        Log.Information($"MaxDaysToStorageBackup: {Program.MaxDaysToStorageBackup}");

        var HoursOfDayToRunBackup = new List<int>();
        Configuration.GetSection("HoursOfDayToRunBackup").Bind(HoursOfDayToRunBackup);
        Program.HoursOfDayToRunBackup = HoursOfDayToRunBackup;
        Log.Information($"HoursOfDayToRunBackup: {string.Join(",", Program.HoursOfDayToRunBackup)}");

        var sqlConfig = new SQLServerConfig();
        Configuration.GetSection("SQLServerConfig").Bind(sqlConfig);
        Program.sqlConfig = sqlConfig;
        Log.Information($"SQLServerConfig: {Newtonsoft.Json.JsonConvert.SerializeObject(sqlConfig)}");
    }
}


