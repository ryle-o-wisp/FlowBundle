using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddRazorPages();
builder.Services.AddDirectoryBrowser();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

var contentFileProvider = new FileExtensionContentTypeProvider();
// Add Unity files
contentFileProvider.Mappings[".bundle"] = "application/x-binary";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = contentFileProvider,
});

app.UseRouting();

app.Run();
