using Azure.Identity;
using DocumentSearchPortal.Models;
using DocumentSearchPortal.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Azure Key Vault to configuration builder  
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
    new DefaultAzureCredential());

// Add services to the container.  
builder.Services.AddControllersWithViews();

// Register the SearchService with the necessary parameters using DI.  
builder.Services.AddSingleton<SearchService>((s) =>
{
    // Use the IOptions pattern to access the SearchServiceConfig  
    var config = s.GetRequiredService<IConfiguration>();
    var searchServiceConfig = config.GetSection("SearchService").Get<SearchServiceConfig>();

    // Ensure that all configuration settings have been retrieved successfully  
    if (searchServiceConfig.ServiceName == null ||
        searchServiceConfig.IndexName == null ||
        searchServiceConfig.ApiKey == null)
    {
        throw new InvalidOperationException("Search service configuration is not set correctly.");
    }

    return new SearchService(
        searchServiceConfig.ServiceName,
        searchServiceConfig.IndexName,
        searchServiceConfig.ApiKey
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.  
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Search/Error");
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
