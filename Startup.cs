using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EF_DotNetCore.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace EF_DotNetCore
{
    public class Startup
    {
        IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config=config;
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {   
            services.AddDbContextPool<EmployessDBContext>(options=>options.UseSqlServer(_config.GetConnectionString("EmployeeDBConnection")));
            services.AddMvc(options=>options.EnableEndpointRouting=false);
            services.AddScoped<IMockEmployeeRepository, SQLEmployeeRepository>();
            services.AddIdentity<ApplicaitonUsers, IdentityRole>().AddEntityFrameworkStores<EmployessDBContext>();
            services.AddMvc(cofig =>
            {
                var policy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                cofig.Filters.Add(new AuthorizeFilter(policy));
            });
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
                app.UseExceptionHandler("/Error"); 
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }
            app.UseStaticFiles();
            app.UseAuthentication();
            
            app.UseMvc(routes=>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
           

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {
            //        await context.Response.WriteAsync("Hello World!");
            //    });
            //});
            //app.UseMvcWithDefaultRoute();
            
        }
    }
}
