using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using ASC.Web.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ASC.Web.Configuration;
using ElCamino.AspNetCore.Identity.AzureTable.Model;
using IdentityUser = ElCamino.AspNetCore.Identity.AzureTable.Model.IdentityUser;
using ASC.Web.Models;
using Microsoft.Extensions.Options;
using IdentityRole = ElCamino.AspNetCore.Identity.AzureTable.Model.IdentityRole;
using Microsoft.AspNetCore.Http;
using ASC.Web.Service;

namespace ASC.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Elcamino Azure Table Identity services.
            //services.AddDbContext(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Set whatever identity options here
                options.SignIn.RequireConfirmedAccount = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddAzureTableStores<ApplicationDbContext>(new Func<IdentityConfiguration>(() =>
            {
                IdentityConfiguration idconfig = new IdentityConfiguration();
                idconfig.TablePrefix = Configuration.GetSection("IdentityAzureTable:IdentityConfiguration:TablePrefix").Value;
                idconfig.StorageConnectionString = Configuration.GetSection("IdentityAzureTable:IdentityConfiguration:StorageConnectionString").Value;
                idconfig.LocationMode = Configuration.GetSection("IdentityAzureTable:IdentityConfiguration:LocationMode").Value;
                //Setting TableNames is optional, default value shown below if not set.
                idconfig.IndexTableName = Configuration.GetSection("IdentityAzureTable:IdentityConfiguration:IndexTableName").Value; // default: AspNetIndex
                idconfig.RoleTableName = Configuration.GetSection("IdentityAzureTable:IdentityConfiguration:RoleTableName").Value;   // default: AspNetRoles
                idconfig.UserTableName = Configuration.GetSection("IdentityAzureTable:IdentityConfiguration:UserTableName").Value;   // default: AspNetUsers

                return idconfig;
            }))
            .AddDefaultTokenProviders()
            .CreateAzureTablesIfNotExists<ApplicationDbContext>(); //can remove after first run;

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddOptions();
            services.Configure<ApplicationSettings>(Configuration.GetSection("AppSettings"));

            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    IConfigurationSection googleAuthNSection = Configuration.GetSection("Authentication:Google");

                    options.ClientId = Configuration["Google:Identity:ClientId"];
                    options.ClientSecret = Configuration["Google:Identity:ClientSecret"];
                })
                .AddFacebook(options =>
                {
                    IConfigurationSection facebookAuthNSection = Configuration.GetSection("Authentication:Facebook");

                    options.ClientId = Configuration["Facebook:Identity:ClientId"];
                    options.ClientSecret = Configuration["Facebook:Identity:ClientSecret"];
                });

            services.AddSingleton<IIdentitySeed, IdentitySeed>();
            // Resolve HttpContextAccessor dependency to access HttpContext in views
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IEmailSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env, IIdentitySeed storageSeed)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            //Create a scope to resolve dependencies registered as scoped
            using (var scope = app.ApplicationServices.CreateScope())
            {
                //Resolve ASP .NET Core Identity with DI help
                var userManager = (UserManager<ApplicationUser>)scope.ServiceProvider.GetService(typeof(UserManager<ApplicationUser>));
                var roleManager = (RoleManager<IdentityRole>)scope.ServiceProvider.GetService(typeof(RoleManager<IdentityRole>));
                var options = (IOptions<ApplicationSettings>)scope.ServiceProvider.GetService(typeof(IOptions<ApplicationSettings>));

                // do you things here
                await storageSeed.Seed(userManager, roleManager, options);
            }
        }
    }
}
