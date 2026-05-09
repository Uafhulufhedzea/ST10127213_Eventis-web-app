using Azure.Storage.Blobs;
using Eventis_web_app.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Replace 'YourProjectNamespace' with the actual namespace in your Models folder
builder.Services.AddDbContext<Eventis_web_app.Models.EventisContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Azure Blob Storage service
var azureBlobConnectionString = builder.Configuration.GetSection("AzureBlobStorage")["ConnectionString"];
builder.Services.AddSingleton(_ =>
    new BlobServiceClient(azureBlobConnectionString));
builder.Services.AddScoped<BlobStorageService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
// Temporarily show detailed errors for debugging
app.UseDeveloperExceptionPage();

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
