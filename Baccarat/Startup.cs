using Baccarat.Hubs;
using Baccarat.Manipulate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Quartz;
using Quartz.Impl;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;

namespace Baccarat
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
            services.AddMvc(options =>
            {
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
             .AddJsonOptions(options => { options.SerializerSettings.ContractResolver = new DefaultContractResolver(); });

            services.AddSignalR(option =>
            {
                option.KeepAliveInterval = TimeSpan.FromSeconds(20);
                option.HandshakeTimeout = TimeSpan.FromSeconds(20);
            });

            services.AddCors(op =>
            {
                op.AddPolicy("AllowAllOrigin", set =>
                {
                    set.SetIsOriginAllowed(origin => true).
                    AllowAnyHeader().
                    AllowAnyMethod().
                    AllowCredentials();
                });
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "AgentManagement API"
                });

                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                var xmlPath = Path.Combine(basePath, "Baccarat.xml");
                options.IncludeXmlComments(xmlPath);
            });

            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //跨域支持
            app.UseCors("AllowAllOrigin");
            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/sign/chatHub");
            });
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };
            app.UseWebSockets(webSocketOptions);
            //swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Test API");
                //配置swagger路径，配置成根路径就是string.Empty;
                c.RoutePrefix = "swagger";// string.Empty;
            });
            app.UseMiddleware<Monitor>();
            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
