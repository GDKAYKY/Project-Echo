// Make the program async
await Main();

async Task Main()
{
    var builder = WebApplication.CreateBuilder(args);

    // Configure port for Render
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

    // Add services to the container.
    builder.Services.AddRazorPages();
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