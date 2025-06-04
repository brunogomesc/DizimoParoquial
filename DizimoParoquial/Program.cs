using DizimoParoquial.Data.Interface;
using DizimoParoquial.Data.Repositories;
using DizimoParoquial.Services;
using DizimoParoquial.Utils;
using NToastNotify;
using Serilog;
using Serilog.Events;


var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .Build();


Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext() 
    .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information) // Adiciona um sink de console para feedback imediato
    .CreateLogger();

try
{

    Log.Information("Iniciando a aplicação web.");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    Log.Information("Configurando o NToastNotify.");
    builder.Services.AddMvc().AddNToastNotifyToastr(new ToastrOptions()
    {
        ProgressBar = false,
        PositionClass = ToastPositions.TopCenter,
        TimeOut = 5000
    });

    Log.Information("Configurando a sessão.");
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromHours(3);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    //Injecting the services
    //Singleton
    builder.Services.AddSingleton<ConfigurationService>();
    builder.Services.AddSingleton<Encryption>();

    //Transient
    builder.Services.AddTransient<UserService>();
    builder.Services.AddTransient<IUserRepository, UserRepository>();

    builder.Services.AddTransient<PermissionService>();
    builder.Services.AddTransient<IPermissionRepository, PermissionRepository>();

    builder.Services.AddTransient<AgentService>();
    builder.Services.AddTransient<IAgentRepository, AgentRepository>();

    builder.Services.AddTransient<TithePayerService>();
    builder.Services.AddTransient<ITithePayerRepository, TithePayerRepository>();

    builder.Services.AddTransient<TitheService>();
    builder.Services.AddTransient<ITitheRepository, TitheRepository>();

    builder.Services.AddTransient<IncomeService>();
    builder.Services.AddTransient<IIncomeRepository, IncomeRepository>();

    builder.Services.AddTransient<EventService>();
    builder.Services.AddTransient<IEventRepository, EventRepository>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseSession();

    app.UseAuthorization();

    app.UseNToastNotify();

    app.MapControllerRoute(
        name: "Login",
        pattern: "{controller=Login}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host encerrado inesperadamente!");
}
finally
{
    Log.CloseAndFlush();
}
