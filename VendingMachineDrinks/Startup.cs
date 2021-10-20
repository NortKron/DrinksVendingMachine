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

using System.Diagnostics;
using VendingMachineDrinks.Models;

namespace VendingMachineDrinks
{
    public class Startup
    {
        private const string CFG_DATA_DB = @"ConnectionStrings:DefaultConnection";
        private const string CFG_ADMIN_ACCESSKEY = @"AdminKeyAccess";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<DataContext>(options => options.UseSqlServer(
                Configuration[CFG_DATA_DB].Replace("%CONTENTROOTPATH%", Environment.CurrentDirectory)) );
            //services.AddDbContext<DataContext>();
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
            //app.UseAuthorization();

            app.UseStatusCodePages();
            //app.UseStatusCodePagesWithRedirects("/");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute
                (
                    name: "default",
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
                        //controller = "Coins",
                        action = "Index"
                    }
                );
                /*
                endpoints.MapControllerRoute
                (
                    name: "default",
                    pattern: Configuration[CFG_ADMIN_ACCESSKEY],
                    defaults: new
                    {
                        controller = "Admin",
                        action = "Index"
                    }
                );
                
                endpoints.MapControllerRoute
                (
                    name: "default",
                    pattern: Configuration[CFG_ADMIN_ACCESSKEY] + "/{action=Index}/{id?}",
                    defaults: new
                    {
                        controller = "Admin",
                        action = "Index"
                    }
                );
                */
            });
        }
    }
}
