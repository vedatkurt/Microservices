using FreeCourse.Gateway.DelegateHandlers;
using FreeCourse.Shared.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Gateway
{
    public class Startup
    {
        private readonly IConfiguration Configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication().AddJwtBearer("GatewayAuthenticationSchema", options=> {
                options.Authority = Configuration["IdentityServerURL"];
                options.Audience = "resource_gateway";
                options.RequireHttpsMetadata = false;
            });

            
            // 193. ders eklendi
            services.AddOcelot().AddDelegatingHandler<TokenExchangeDelegateHandler>();

            services.AddHttpClient<TokenExchangeDelegateHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        async public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            await app.UseOcelot();

        }
    }
}
