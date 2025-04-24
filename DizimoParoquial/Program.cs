using DizimoParoquial.Data.Interface;
using DizimoParoquial.Data.Repositories;
using DizimoParoquial.Services;
using DizimoParoquial.Utils;
using NToastNotify;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddMvc().AddNToastNotifyToastr(new ToastrOptions()
{
    ProgressBar = false,
    PositionClass = ToastPositions.TopCenter,
    TimeOut = 5000
});

builder.Services.AddSession( options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
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
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
