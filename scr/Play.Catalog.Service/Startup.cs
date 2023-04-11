
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Play.Catalog.Service.Repositories;
using Play.Catalog.Service.Contracts;
using Play.Common;
using MassTransit;
using Play.Catalog.Service.Settings;

namespace Play.Catalog.Service
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
            var serviceName = Configuration["ServiceSettings:ServiceName"];
            
            services.AddControllers(opt =>
            {
                opt.SuppressAsyncSuffixInActionNames = false;
            });
            //RabbitMQ settings and configurations
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context,configurator)=>{
                    var rabbitMQSettings =Configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                    configurator.Host(rabbitMQSettings.Host);
                    configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceName,false));
                });
            });

            services.AddMassTransitHostedService();
            
            services.AddDbContext<ApplicationContext>(opt => opt.UseSqlite(Configuration.GetConnectionString("Conn")));
            services.AddControllers();
            services.AddScoped<IItemsRepository, ItemsRepository>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Play.Catalog.Service", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Play.Catalog.Service v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
