using Ganss.Xss;
using IntelliTest.Core.Contracts;
using IntelliTest.Data;
using IntelliTest.Data.Entities;
using IntelliTest.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IntelliTest.Core.Models.Mails;
using System.Text.Json.Serialization;
using IntelliTest.Core.Hubs;
using IntelliTest.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("all", opt =>
    {
        opt.AllowAnyOrigin();
        opt.AllowAnyHeader();
        opt.AllowAnyMethod();
    });
});


var connectionString = builder.Configuration["ConnectionString"];
builder.Services.AddDbContext<IntelliTestDbContext>(options =>
                                                            options.UseSqlServer(connectionString)
                                                            ,ServiceLifetime.Singleton);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;
})
       .AddEntityFrameworkStores<IntelliTestDbContext>()
       .AddDefaultTokenProviders();

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
}).AddJsonOptions(o => o.JsonSerializerOptions
                        .ReferenceHandler = ReferenceHandler.Preserve);

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/User/Login";
    options.AccessDeniedPath = "/Error/401";
});

builder.Services.AddControllers()
       .AddJsonOptions(x => 
                           x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
       .AddRazorOptions(options =>
       {
           options.AreaViewLocationFormats.Add("/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
       });

builder.Services.AddSingleton<IMessageService, MessageService>();
builder.Services.AddSingleton<IRoomService, RoomService>();
builder.Services.AddSingleton<ITestService, TestService>();
builder.Services.AddSingleton<ITestResultsService, TestResultsService>();
builder.Services.AddSingleton<ITeacherService, TeacherService>();
builder.Services.AddSingleton<ILessonService, LessonService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<IClassService, ClassService>();
builder.Services.AddSingleton<IStudentService, StudentService>();

builder.Services.AddSignalR();

//builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache();
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

using var scope = app.Services.CreateScope();
var roleManager = (RoleManager<IdentityRole>)scope.ServiceProvider.GetService(typeof(RoleManager<IdentityRole>));
string[] roleNames = { "Teacher", "Student" };
IdentityResult roleResult;
app.SeedAdmin();

foreach (var roleName in roleNames)
{
    var roleExist = await roleManager.RoleExistsAsync(roleName);
    if (!roleExist)
    {
        //create the roles and seed them to the database: Question 1
        roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
    }
}

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

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "Areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
    endpoints.MapControllerRoute(
        name: "Tests",
        pattern: "{controller}/{action}/{id?}");
    endpoints.MapDefaultControllerRoute();
    endpoints.MapRazorPages();
});
app.MapHub<ChatHub>("/chatHub");
app.MapHub<TestEditHub>("/testEditHub");

app.Run();
//TODO:Check ofr quotes in questions

///anti XSS cross site scripting
//var sanitizer = new HtmlSanitizer();
//sanitizer.Sanitize("");
//Test Image editing
//TODO: TestController 175 test image add