using Ganss.Xss;
using IntelliTest.Core.Contracts;
using IntelliTest.Data;
using IntelliTest.Data.Entities;
using IntelliTest.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Microsoft.AspNetCore.Builder;
using MailKit;
using IntelliTest.Core.Models.Mails;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration["ConnectionString"];
builder.Services.AddDbContext<IntelliTestDbContext>(options =>
                                                        options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;
})
       .AddRoles<IdentityRole>()
       .AddEntityFrameworkStores<IntelliTestDbContext>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("all", opt =>
    {
        opt.AllowAnyOrigin();
        opt.AllowAnyMethod();
    });
});


//External logins
builder.Services.AddAuthentication()
       .AddFacebook(facebookOptions =>
       {
           facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
           facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
       }).AddGoogle(googleOptions =>
       {
           googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
           googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
       });

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/User/Login";
});

builder.Services.AddControllersWithViews();

builder.Services.AddTransient<ITestService, TestService>();
builder.Services.AddTransient<IStudentService, StudentService>();
builder.Services.AddTransient<ITeacherService, TeacherService>();
builder.Services.AddTransient<ILessonService, LessonService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
});

//against CSRF Cross-Site Request Forgery
builder.Services.AddMvc(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));
builder.Services.Configure<DataProtectionTokenProviderOptions>(o =>
                                                                   o.TokenLifespan = TimeSpan.FromHours(3));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    //app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseDeveloperExceptionPage();
app.UseMigrationsEndPoint();

app.UseCors("all");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
//Use different way to store connection string in production

//TODO:Check ofr quotes in questions

///anti XSS cross site scripting
//var sanitizer = new HtmlSanitizer();
//sanitizer.Sanitize("");