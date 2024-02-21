using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using DocumentSearchPortal.Services;
using Microsoft.Extensions.Configuration;
using DocumentSearchPortal.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Bind the SearchServiceOptions  
var searchServiceConfig = new SearchServiceConfig();
builder.Configuration.GetSection("SearchService").Bind(searchServiceConfig);

// Register the SearchService with the necessary parameters.
// As these are necessary config values, We assume that these will always have values in the config file.
builder.Services.AddSingleton(s =>
    new SearchService(
        searchServiceConfig.ServiceName!,
        searchServiceConfig.IndexName!, 
        searchServiceConfig.ApiKey!
    ));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Search/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.  
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Search}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
