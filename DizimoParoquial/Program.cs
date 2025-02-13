using DizimoParoquial.Services;
using NToastNotify;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();

//Injecting the services

builder.Services.AddSingleton<ConfigurationService>();

builder.Services.AddMvc().AddNToastNotifyToastr( new ToastrOptions()
{
    ProgressBar = false,
    //CloseButton = true,
    PositionClass = ToastPositions.TopCenter,
    TimeOut = 7000
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseNToastNotify();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
