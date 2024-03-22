using Azure.Identity;
using DocumentSearchPortal.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using DocumentSearchPortal.Data;
using DocumentSearchPortal.Services.Search;
using DocumentSearchPortal.Services.Upload;
using Azure.Search.Documents.Indexes;
using Azure;

var builder = WebApplication.CreateBuilder(args);

// Add Azure Key Vault to configuration builder  
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
    new DefaultAzureCredential());

builder.Services.Configure<SearchServiceConfig>(builder.Configuration.GetSection("SearchService"));


// Add services to the container.  
builder.Services.AddControllersWithViews();

// Add scoped services for Search and Upload functionalities  
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IUploadService, UploadService>();

// Register ApplicationDbContext using the DbConnectionString from SearchServiceConfig  
builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
{
    SearchServiceConfig searchServiceConfig = serviceProvider.GetRequiredService<IOptions<SearchServiceConfig>>().Value;
    options.UseSqlServer(searchServiceConfig.SQLDbConnectionString);
});

WebApplication app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
    app.UseDeveloperExceptionPage();
//}
//else
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
