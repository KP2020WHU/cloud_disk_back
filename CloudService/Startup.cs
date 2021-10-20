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
            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null); // ���Json���л����ã��������л�ʱ������������ĸ��formatΪСд

            services.AddScoped<UserManage, UserManage>();
            services.AddScoped<CloudFileManage, CloudFileManage>();

            // configure strongly typed settings object
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            // �� ������ϵע�루DI����������ע�����ݿ�������
            services.AddDbContextPool<CloudFileContext>(options => options
            .UseMySql(Configuration.GetConnectionString("CloudFileApiDatabase"), // ʹ�õ������ַ���
            new MySqlServerVersion(new Version(8, 0, 21))));

            services.AddDbContextPool<UserContext>(options => options
            .UseMySql(Configuration.GetConnectionString("UserApiDatabase"), // ʹ�õ������ַ���
            new MySqlServerVersion(new Version(8, 0, 21))));


            // ��ӿ���������ã�ǰ�����ȫ���룩
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
