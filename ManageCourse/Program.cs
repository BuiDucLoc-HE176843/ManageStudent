using ManageCourse.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<LearningManagementSystemContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DB")));

// Cấu hình session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromMinutes(30);
    opt.Cookie.HttpOnly = true;
    opt.Cookie.IsEssential = true;
});

// Cấu hình Authentication bằng Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Login"; // Đường dẫn khi người dùng chưa đăng nhập
        options.AccessDeniedPath = "/Login/Warning"; // Đường dẫn khi người dùng không có quyền truy cập
    });

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Sử dụng authentication và authorization
app.UseAuthentication(); // Để sử dụng authentication
app.UseAuthorization();  // Để sử dụng authorization

app.MapRazorPages();

app.Run();
