using DizimoParoquial.Data.Interface;
using DizimoParoquial.Data.Repositories;
using DizimoParoquial.Services;
using DizimoParoquial.Utils;
using NToastNotify;
using Serilog;
using Serilog.Events;

try
{

    Log.Information("Iniciando a aplicação web.");

    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();

    //var resourceBuilder = ResourceBuilder.CreateDefault()
    //.AddService("OrderProcessingService")
    //.AddAttributes(new Dictionary<string, object>
    //{
    //    ["environment"] = "development",
    //    ["service.version"] = "1.0.0"
    //});

    //builder.Logging.AddOpenTelemetry(logging => {
    //    logging.IncludeFormattedMessage = true;
    //    logging.SetResourceBuilder(resourceBuilder)
    //        .AddOtlpExporter(otlpOptions => {
    //            otlpOptions.Endpoint = new Uri("https://monitoria.devcorehub.com.br/api/default/v1/logs");
    //            otlpOptions.Headers = "Authorization=Basic ZGV2Y29yZWh1YkBnbWFpbC5jb206Vkl4ZkJaVE4xMmFiTVEzMg==";
    //            otlpOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
    //        });
    //});

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
