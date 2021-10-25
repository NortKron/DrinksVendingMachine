using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;

using VendingMachineDrinks.Models;
using VendingMachineDrinks.Controllers;

namespace VendingMachineDrinks
{
    public class Startup
    {
        private const string CFG_DATA_DB = @"ConnectionStrings:DefaultConnection";
        private const string CFG_ADMIN_ACCESSKEY = @"AdminKeyAccess";
        
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            /*
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            */
            //Configuration = builder.Build();
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<DataContext>(options => options.UseSqlServer(
                Configuration[CFG_DATA_DB].Replace("%CONTENTROOTPATH%", Environment.CurrentDirectory)) );
            //services.AddDbContext<DataContext>();

            services.Configure<MvcOptions>(options =>
            {
                //mvc options
            });

            //Debug.Print(">>>> admin key : " + Configuration.GetSection(CFG_ADMIN_ACCESSKEY).Value);
            services.Configure<AppSettings>(Configuration.GetSection(CFG_ADMIN_ACCESSKEY));
            //services.Configure<AppSettings>(Configuration[CFG_ADMIN_ACCESSKEY]);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            //app.UseStatusCodePages();
            app.UseStatusCodePagesWithRedirects("/");

            app.UseEndpoints(endpoints =>
            {
                
                endpoints.MapControllerRoute
                (
                    name: "Admin",
                    pattern: Configuration[CFG_ADMIN_ACCESSKEY] + "/{action=Index}/{id?}",
                    defaults: new
                    {
                        controller = "Admin",
                        action = "Index"
                    }
                );
                
                endpoints.MapControllerRoute
                (
                    name: "Home",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );
                /*
                endpoints.MapControllerRoute
                (
                    name: "default",
                    pattern: "",
                    defaults: new
                    {
                        controller = "Home",
                        action = "Index"
                    }
                );
                */                
            });
        }
    }
}
