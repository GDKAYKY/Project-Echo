using Project_Echo.Services.Navigation;
using Project_Echo.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

// Make the program async
await Main();

async Task Main()
{
    var builder = WebApplication.CreateBuilder(args);

    // Configure port for Render
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

    // Configure database storage path
    var databaseStoragePath = Environment.GetEnvironmentVariable("DatabaseStoragePath") ?? 
                            Path.Combine(builder.Environment.ContentRootPath, "database_storage");
    builder.Configuration["DatabaseStoragePath"] = databaseStoragePath;

    // Add services to the container.
    builder.Services.AddRazorPages()
        .AddRazorRuntimeCompilation();

    // Add support for controllers
    builder.Services.AddControllersWithViews();

    // Register terminal service
    builder.Services.AddSingleton<Project_Echo.Services.TerminalService>();

    // Add session state for terminal session
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromHours(1);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    // Register navigation services
    builder.Services.AddScoped<ISidebarService, SidebarService>();

    // Configure file upload size limits
    builder.Services.Configure<IISServerOptions>(options =>
    {
        options.MaxRequestBodySize = 300 * 1024 * 1024; // 300 MB
    });

    builder.Services.Configure<KestrelServerOptions>(options =>
    {
        options.Limits.MaxRequestBodySize = 300 * 1024 * 1024; // 300 MB
    });

    builder.Services.Configure<FormOptions>(options =>
    {
        options.MultipartBodyLengthLimit = 300 * 1024 * 1024; // 300 MB
    });

    // Register database services
    builder.Services.AddScoped<IDatabaseService, DatabaseService>();
    builder.Services.AddScoped<IDatabaseQueryService, DatabaseQueryService>();
    builder.Services.AddScoped<IDatabaseTypeDetector, DatabaseTypeDetector>();

    // Add logging
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Logging.AddDebug();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();
    app.UseAuthorization();

    // Enable session
    app.UseSession();

    app.MapRazorPages();
    // Map controller routes
    app.MapControllers();

    await app.RunAsync();
} 