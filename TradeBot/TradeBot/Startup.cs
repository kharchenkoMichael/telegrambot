using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TradeBot.Models;
using TradeBot.Servecies;

namespace TradeBot
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<BotContext>();

            services.AddMvc();
            services.AddSingleton<Bot>();
            services.AddSingleton<AppSettings>();
        }

       public void Configure(IApplicationBuilder app, IHostingEnvironment env, Bot bot)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            bot.Get();
            app.UseMvcWithDefaultRoute();
        }
    }
}
