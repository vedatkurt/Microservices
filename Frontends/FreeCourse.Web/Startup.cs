using FluentValidation.AspNetCore;
using FreeCourse.Shared.Services;
using FreeCourse.Web.Extensions;
using FreeCourse.Web.Handler;
using FreeCourse.Web.Helpers;
using FreeCourse.Web.Models;
using FreeCourse.Web.Models.Catalog;
using FreeCourse.Web.Services;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {            
            // 113.Ders
            // Options pattern
            services.Configure<ClientSettings>(Configuration.GetSection("ClientSettings"));
            services.Configure<ServiceApiSettings>(Configuration.GetSection("ServiceApiSettings"));

            // 117.Ders
            services.AddHttpContextAccessor();

            // 132. ders
            // SharedIdentityService eklenir
            services.AddScoped<ISharedIdentityService, SharedIdentityService>();
            services.AddAccessTokenManagement();
            services.AddSingleton<PhotoHelper>();

            // 133. ders
            // ClientCredentialTokenService eklenir
            services.AddScoped<IClientCredentialTokenService, ClientCredentialTokenService>();

            services.AddScoped<ClientCredentialTokenHandler>();

            services.AddScoped<ResourceOwnerPasswordTokenHandler>();

            // static bir Extension method yazildi.
            services.AddHttpClientServices(Configuration);

            // 118.Ders
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
                CookieAuthenticationDefaults.AuthenticationScheme,optios =>
                {
                    optios.LoginPath = "/Auth/SignIn";
                    optios.ExpireTimeSpan = TimeSpan.FromDays(60);
                    optios.SlidingExpiration = true;
                    optios.Cookie.Name = "udemywebcookie";
                });

            services.AddControllersWithViews().AddFluentValidation(fv=> fv.RegisterValidatorsFromAssemblyContaining<CourseCreateInput>());
            services.AddControllersWithViews().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CourseUpdateInput>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
