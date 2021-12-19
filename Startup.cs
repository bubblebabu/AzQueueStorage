using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using nsWeatherDataService;
using Azure.Identity;
namespace AzQueueStorage
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
            services.AddAzureClients(builder => {
                builder.AddClient<QueueClient, QueueClientOptions>((_, _, _) =>
                {
                    var credentails = new DefaultAzureCredential();
                    var uri = new Uri("https://bvbstorage1.queue.core.windows.net/bvb-queue");
                    /*string connectionString = "DefaultEndpointsProtocol=https;AccountName=bvbstorage1;AccountKey=xtQCLJfK0qE8/LYYOgw+uiILg95yp9KxsQHFLUyC/e6Vebmm/+iTbh68P71zssVBDq5csVa95HiG+rAnMzcUQA==;EndpointSuffix=core.windows.net";
                    string queueName = "bvb-queue";*/
                    return new QueueClient(uri, credentails);
                });
            });
            services.AddControllers();
            services.AddHostedService<WeatherDataService>();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AzQueueStorage", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AzQueueStorage v1"));
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
