using DataModel.Models;
using Microsoft.EntityFrameworkCore;
using Repository;
using ViewModel;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorPages();
builder.Services.AddDbContext<FuminiHotelManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<CustomerViewModel>();
builder.Services.AddScoped<CustomerRepository>();
builder.Services.AddScoped<BookingReservationRepository>();
builder.Services.AddAuthentication().AddCookie("MyCookie", options =>
{
    options.Cookie.Name = "MyCookie";
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Error";
});

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

app.MapRazorPages();
app.MapFallbackToPage("/Account/Login");

app.Run();
