using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CloudService.Models;
using Newtonsoft.Json.Serialization;
using CloudService.Helpers;
using CloudService.BLL;
using Microsoft.AspNetCore.Http;

namespace CloudService
{
    public class Startup
    {

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null); // 添加Json序列化设置，避免序列化时键的名称首字母被format为小写

            services.AddScoped<UserManage, UserManage>();
            services.AddScoped<CloudFileManage, CloudFileManage>();

            // configure strongly typed settings object
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            // 向 依赖关系注入（DI）容器进行注册数据库上下文
            services.AddDbContextPool<CloudFileContext>(options => options
            .UseMySql(Configuration.GetConnectionString("CloudFileApiDatabase"), // 使用的连接字符串
            new MySqlServerVersion(new Version(8, 0, 21))));

            services.AddDbContextPool<UserContext>(options => options
            .UseMySql(Configuration.GetConnectionString("UserApiDatabase"), // 使用的连接字符串
            new MySqlServerVersion(new Version(8, 0, 21))));


            // 添加跨域访问设置（前后端完全分离）
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.WithOrigins("http://localhost:7700"
                                          , "https://localhost:7700")
                                      .AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                                  }); // no trailing slash!
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins); // has to be after UseRouting, but before UseAuthorization

            app.UseAuthorization();

            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }
    }
}
