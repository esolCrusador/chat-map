using ChatMap.Entities;
using ChatMap.Infrastructure;
using ChatMap.Infrastructure.Contracts;
using ChatMap.Repositories;
using ChatMap.Services;
using ChatMap.Services.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatMap
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMvc();

            services.AddSingleton(SetupConfiguration());
            services.AddSingleton<ConfigurationService>();

            services.AddSingleton<IDownloadClientsPool, DownloadClientsPool>();
            services.AddTransient<ISocialNetworkStrategy, InstagramSocialNetworkStrategy>();
            services.AddSingleton<SocialNetworkResolver>();
            services.AddTransient<GoogleApiClient>();

            services.AddSingleton<DynamicMapper>();
            services.AddTransient<IChatRepository, ChatRepository>();
            services.AddTransient<IChatService, ChatService>();
            services.AddSingleton<GoogleSheetShemaProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute("Default", "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public IConfigurationRoot SetupConfiguration()
        {
            ConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddEnvironmentVariables();
            configBuilder.AddInMemoryCollection();
            configBuilder.AddJsonFile("app.config.json", false, true);

            return configBuilder.Build();
        }
    }
}
