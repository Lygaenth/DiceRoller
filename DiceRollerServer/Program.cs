using DiceRoller.Core.Apis;
using DiceRollerServer.Hubs;
using DiceRollerServer.Services;
using Serilog;

class Program
{
    static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder);
        ConfigureLogger(builder);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();
        app.MapHub<RollHub>("/rollHub");

        app.Run();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddSignalR();
        builder.Services.AddLogging();

        builder.Services.AddSingleton<IPartyService, PartyService>();
        builder.Services.AddTransient<IRollService, RollService>();
        builder.Services.AddTransient<IMapService, MapService>();
        builder.Services.AddSingleton<RollHub>();
    }

    private static void ConfigureLogger(WebApplicationBuilder builder)
    {
        var loggerFactory = new LoggerFactory();
        loggerFactory.AddSerilog();

        var logPath = Path.GetDirectoryName(Environment.ProcessPath) + "\\Logs\\DiceRoller.txt";

        var serilogLogger = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
        .CreateLogger();

        builder.Services.AddSingleton(serilogLogger);
    }
}