using DataModel.Models;
using Microsoft.EntityFrameworkCore;
using RazorPage.Hubs;
using Repository;
using ViewModel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddDbContext<FuminiHotelManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<CustomerViewModel>();
builder.Services.AddScoped<EmailSendModel>();
builder.Services.AddScoped<CustomerRepository>();
builder.Services.AddScoped<BookingReservationRepository>();
builder.Services.AddScoped<BookingDetailRepository>();
builder.Services.AddScoped<EmailViewModel>();
builder.Services.AddScoped<WorkerService>();
builder.Services.AddAuthentication().AddCookie("MyCookie", options =>
{
    options.Cookie.Name = "MyCookie";
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Error";
});
builder.Services.AddSignalR();
builder.Services.AddHostedService<WorkerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<SignalRServer>("/signalRServer");

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapFallbackToPage("/Account/Login");
});

app.Run();
