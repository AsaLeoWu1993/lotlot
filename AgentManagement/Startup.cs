using AgentManagement.Manipulate;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;

namespace AgentManagement
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
                options.Filters.Add(new AuthenticationAttribute());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddJsonOptions(options => { options.SerializerSettings.ContractResolver = new DefaultContractResolver(); });

            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "AgentManagement API"
                });

                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                var xmlPath = Path.Combine(basePath, "AgentManagement.xml");
                options.IncludeXmlComments(xmlPath);
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "AgentManagementLoginInfo";
                //两个小时到期时间
                options.Cookie.Expiration = TimeSpan.FromHours(20);
                options.LoginPath = "/api/Cookies/UserLogin";
                options.LogoutPath = "/api/Cookies/UserExit";
            });
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("AllowAllOrigin", policy => policy
            //    .WithOrigins(new string[] { "http://192.168.3.254:9012",
            //    "http://192.168.3.96:8082",
            //    "http://lt.muzhituding.com",
            //    "http://hz.asalogs.com",
            //    "http://cp.asalogs.com"})
            //                            //.AllowAnyOrigin()
            //                            .AllowAnyHeader()
            //                            .AllowAnyMethod()
            //                            .AllowCredentials());
            //});
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
            services.Configure<FormOptions>(x =>
            {
                x.MultipartBodyLengthLimit = 300_000_000;//不到300M
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //访问文件
            app.UseStaticFiles();

            //跨域支持
            app.UseCors("AllowAllOrigin");
            app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });
            app.UseSession();
            //swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Test API");
                //配置swagger路径，配置成根路径就是string.Empty;
                c.RoutePrefix = "swagger";// string.Empty;
            });
            //异常处理中间件
            app.UseMiddleware<ExceptionHandlerMiddleWare>();
            //cookies
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
