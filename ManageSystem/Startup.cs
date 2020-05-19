using AspNetCoreRateLimit;
using Entity;
using Hangfire;
using Hangfire.MemoryStorage;
using ManageSystem.Hubs;
using ManageSystem.Manipulate;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Operation.Common;
using Quartz;
using Quartz.Impl;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ManageSystem
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
                //所有控制器添加特性
                //options.Filters.Add(new AuthenticationAttribute());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddJsonOptions(options => { options.SerializerSettings.ContractResolver = new DefaultContractResolver(); });
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "ManageSystem API"
                });

                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                var xmlPath = Path.Combine(basePath, "ManageSystem.xml");
                options.IncludeXmlComments(xmlPath);
            });

            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            //读取配置
            services.AddOptions();
            // 存储IP计数器及配置规则
            services.AddMemoryCache();
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseStorage(new MemoryStorage()));

            // Add the processing server as IHostedService
            services.AddHangfireServer();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "MerchantLoginInfo";
                //两个小时到期时间
                options.Cookie.Expiration = TimeSpan.FromHours(20);
                options.LoginPath = "/api/Cookies/UserLogin";
                options.LogoutPath = "/api/Cookies/UserExit";
            });
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

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false;//这里要改为false，默认是true，true的时候session无效
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(5);
                options.IOTimeout = TimeSpan.FromHours(5);
                options.Cookie.HttpOnly = true;
            });

            services.AddSingleton<IServiceProvider, ServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
            //app.UseIpRateLimiting();
            //访问文件
            app.UseStaticFiles();
            app.UseSession();
            //swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Test API");
                //配置swagger路径，配置成根路径就是string.Empty;
                c.RoutePrefix = "swagger";// string.Empty;
            });
            //指定Hangfire使用内存存储后台任务信息
            GlobalConfiguration.Configuration.UseMemoryStorage();
            //启用HangfireServer这个中间件（它会自动释放）
            //队列时间
            var options = new BackgroundJobServerOptions
            {
                SchedulePollingInterval = TimeSpan.FromSeconds(1),
                HeartbeatInterval = TimeSpan.FromSeconds(30),
                ServerTimeout = TimeSpan.FromMinutes(5),
                ServerName = "betlottery",
                WorkerCount = Environment.ProcessorCount * 500
                //WorkerCount = int.MaxValue
            };
            app.UseHangfireServer(options);
            //启用Hangfire的仪表盘（可以看到任务的状态，进度等信息）
            app.UseHangfireDashboard();

            //异常处理中间件
            app.UseMiddleware<ExceptionHandlerMiddleWare>();
            app.UseMiddleware<GraspDatas>();

            app.UseIpRateLimiting();

            //cookies
            app.UseAuthentication();
            app.UseMvc();

            //采集数据
            if (Utils.Collect)
            {
                var dic = GameBetsMessage.EnumToDictionary(typeof(GameOfType));
                foreach (var item in dic)
                {
                    var gameType = GameBetsMessage.GetEnumByStatus<GameOfType>(item.Value);
                    GameCollection.StartGameCollect(gameType);
                }
            }
            if (Utils.Variable)
            {
                var tasks = new List<Task>();
                var dic = GameBetsMessage.EnumToDictionary(typeof(GameOfType));
                foreach (var item in dic)
                {
                    var gameType = GameBetsMessage.GetEnumByStatus<GameOfType>(item.Value);
                    tasks.Add(GameCollection.GameStart(gameType)); 
                }
                tasks.Add(GameCollection.OtherThinks());
                tasks.Add(GameCollection.AutoRevise());
                //tasks.Add(Common.GetGameList());
                //tasks.Add(RedisHub.ServerToCompany());
                Task.WaitAll(tasks.ToArray());
            }
        }

    }
}
