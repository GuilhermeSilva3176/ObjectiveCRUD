using Refit;
using Site.Interfaces;
using Site.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRefitClient<IUserNoAuthInterfaces>()
        .ConfigureHttpClient(c => 
        {   
            c.BaseAddress = new Uri("https://localhost:7299/"); 
        });
builder.Services.AddRefitClient<IUserAuthInterfaces>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7299/"))
        .AddHttpMessageHandler<AuthTokenHandler>();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
