using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using VendingMachineDrinks.Models;
using VendingMachineDrinks.Controllers;

namespace VendingMachineDrinks
{
    public class Startup
    {
        private const string CFG_DATA_DB = @"ConnectionStrings:DefaultConnection";
        private const string CFG_ADMIN_ACCESSKEY = @"Admin";
        
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<DataContext>(options => options.UseSqlServer(
                Configuration[CFG_DATA_DB].Replace("%CONTENTROOTPATH%", Environment.CurrentDirectory)) );

            services.Configure<AppSettings>(Configuration.GetSection(CFG_ADMIN_ACCESSKEY));
        }

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

            app.UseStatusCodePagesWithRedirects("/");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
