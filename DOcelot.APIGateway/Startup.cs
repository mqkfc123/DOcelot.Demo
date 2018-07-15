using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using IdentityServer4.AccessTokenValidation;

namespace DOcelot.APIGateway
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

            //IdentityServer Ocelot集成了IdentityServer
            #region IdentityServerAuthenticationOptions => need to refactor
            Action<IdentityServerAuthenticationOptions> isaOptClient = option =>
            {
                option.Authority = Configuration["IdentityService:Uri"];
                //这里的ApiName主要对应于IdentityService中的ApiResource中定义的ApiName
                option.ApiName = "OrderService";
                option.RequireHttpsMetadata = Convert.ToBoolean(Configuration["IdentityService:UseHttps"]);
                option.SupportedTokens = SupportedTokens.Both;
                option.ApiSecret = Configuration["IdentityService:ApiSecrets:OrderService"];
            };
            Action<IdentityServerAuthenticationOptions> isaOptProduct = option =>
            {
                option.Authority = Configuration["IdentityService:Uri"];
                //这里的ApiName主要对应于IdentityService中的ApiResource中定义的ApiName
                option.ApiName = "ProductService";
                option.RequireHttpsMetadata = Convert.ToBoolean(Configuration["IdentityService:UseHttps"]);
                option.SupportedTokens = SupportedTokens.Both;
                option.ApiSecret = Configuration["IdentityService:ApiSecrets:ProductService"];
            };
            #endregion

            services.AddAuthentication()
                .AddIdentityServerAuthentication("OrderServiceKey", isaOptClient)
                .AddIdentityServerAuthentication("ProductServiceKey", isaOptProduct);

            services.AddOcelot(Configuration);
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseOcelot().Wait();
        }
    }
}
