using Microsoft.EntityFrameworkCore;
using SNHU_Capstone_Project.Models.Handlers;
using System;

namespace SNHU_Capstone_Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<MSSQLDatabaseContext>(options => options.UseSqlServer(ConfigurationBinder.GetValue<string>(builder.Configuration, "MSSQLConnectionString")));


            //  Add cookie authentication
            builder.Services.AddAuthentication().AddCookie("LoginCookieAuth", options =>
            {
                options.Cookie.Name = "LoginCookieAuth";
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/Unauthorized";
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("UserIsAdministrator", policy =>
                    policy.RequireClaim("Administrator", "True"));
                options.AddPolicy("PageCustomerAdministration", policy =>
                {
                    policy.RequireAssertion(context =>
                    context.User.HasClaim(c => (c.Type.Equals("PageCustomerAdministration") && c.Value.Equals("True")) || (c.Type.Equals("Administrator") && c.Value.Equals("True"))));
                });
                options.AddPolicy("EditCustomers", policy =>
                {
                    policy.RequireAssertion(context =>
                    context.User.HasClaim(c => (c.Type.Equals("PermissionUpdate") && c.Value.Equals("True")) || (c.Type.Equals("Administrator") && c.Value.Equals("True"))));
                });
                options.AddPolicy("CreateCustomers", policy =>
                {
                    policy.RequireAssertion(context =>
                    context.User.HasClaim(c => (c.Type.Equals("PermissionCreate") && c.Value.Equals("True")) || (c.Type.Equals("Administrator") && c.Value.Equals("True"))));
                });
                options.AddPolicy("DeleteCustomers", policy =>
                {
                    policy.RequireAssertion(context =>
                    context.User.HasClaim(c => (c.Type.Equals("PermissionDelete") && c.Value.Equals("True")) || (c.Type.Equals("Administrator") && c.Value.Equals("True"))));
                });

            });


            var app = builder.Build();


            //  Seed the database
            CreateDbIfNotExists(app);


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        private static void CreateDbIfNotExists(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<MSSQLDatabaseContext>();

            try
            {
                MSSQLDatabaseInitializer.Initialize(databaseContext);
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred initializing the database..");
            }
        }

    }
}
