using CodeGenerator.Common;
using CodeGenerator.DB;
using EntityCreater;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CodeGenerator
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataDbContext>(options => options.UseSqlServer("Data Source=blog.db"));

            services.AddControllersWithViews()
                .AddControllersAsServices(); //将控制器添加为服务

            services.AddSession();

            services.AddHttpContextAccessor();
            //在ConfigureServices中注册缓存服务
            services.AddMemoryCache();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddScoped<CreateDefaultBuilder>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //赋值给静态类方便手动获取依赖注入对象
            MyServiceProvider.ServiceProvider = app.ApplicationServices;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseSession();

            app.UseHttpsRedirection();

            #region 静态文件

            app.UseStaticFiles();

            //// 映射路径一
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(
            //        Path.Combine(env.ContentRootPath, "Plugins")),
            //    RequestPath = "/Plugins"
            //});

            //// 映射路径二
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "Content")),
            //    RequestPath = "/Content",
            //});

            //// 映射路径三
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "Images")),
            //    RequestPath = "/Images",
            //});

            //// 映射路径四
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "Scripts")),
            //    RequestPath = "/Scripts",
            //});

            //// 映射路径五
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "fonts")),
            //    RequestPath = "/fonts",
            //});

            #endregion 静态文件

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}