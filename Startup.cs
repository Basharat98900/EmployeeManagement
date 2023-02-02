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
using EF_DotNetCore.Security;
using Microsoft.AspNetCore.Authorization;

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
            services.AddDbContextPool<EmployessDBContext>(options => options.UseSqlServer(_config.GetConnectionString("EmployeeDBConnection")));
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddAuthorization(options =>
            {

            options.AddPolicy("DeleteRolePolicy", policy =>
            policy.RequireAssertion(context => context.User.IsInRole("Admin") &&
            context.User.HasClaim(c => c.Type == "Delete Role" && c.Value == "true") ||
            context.User.IsInRole("SuperAdmin")));

            options.AddPolicy("CreateRolePolicy", policy =>
            policy.RequireAssertion(context => context.User.IsInRole("Admin") &&
            context.User.HasClaim(c => c.Type == "Create Role" && c.Value == "true") ||
            context.User.IsInRole("SuperAdmin")));

                options.AddPolicy("EditRolePolicy", policy =>
                policy.RequireAssertion(context => context.User.IsInRole("Admin") &&
                context.User.HasClaim(c => c.Type == "Edit Role" && c.Value == "true") ||
                context.User.IsInRole("SuperAdmin")));

            });

            services.AddAuthentication()
                .AddGoogle(options =>
            {
                options.ClientId = "837758499523-pvoofkhtmq4uc0v9pobqg6hff6f1h5md.apps.googleusercontent.com";
                options.ClientSecret = "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";
            })
                .AddFacebook(options =>
                {
                    options.ClientId = "1564147514034376";
                    options.ClientSecret = "8fe27ab83b3f6b279487d83f29d496f9";
                }
                
            );
            
                


            services.AddAuthorization(options =>
            {
                options.AddPolicy("RolePolicy", policy =>
                    policy.AddRequirements(new ManageAdminRolesAndClaimRequirement()));
            });
            services.AddSingleton<IAuthorizationHandler, ManageSuperAdmin>();
            services.AddSingleton<IAuthorizationHandler,
                CanEditOnlyOtherAdminRolesAndClaimsHandler>();

            //services.AddAuthorization(options =>
            //options.AddPolicy("EditRolePolicy",policy=>
            //policy.AddRequirements(new ManageAdminRolesAndClaimRequirement()))

            //);
            //services.AddSingleton<IAuthorizationHandler,CanEditOnlyOtherAdminRolesAndClaimsHandler>();
            services.AddScoped<IMockEmployeeRepository, SQLEmployeeRepository>();
            services.AddIdentity<ApplicaitonUsers, IdentityRole>(options=>
            {
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                //options.SignIn.RequireConfirmedEmail = true;
            }
                
                ).AddEntityFrameworkStores<EmployessDBContext>().AddDefaultTokenProviders()
                .AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicaitonUsers>>("");

            services.Configure<DataProtectionTokenProviderOptions>(o =>
            {
                o.TokenLifespan = TimeSpan.FromHours(6);
            }

            );
            services.Configure<CustomEmailConfirmationTokenProviderOptions>(o =>
            o.TokenLifespan = TimeSpan.FromDays(3));
            services.AddMvc(cofig =>
            {
                var policy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                cofig.Filters.Add(new AuthorizeFilter(policy));
            });
            services.AddSingleton<DataProtectionPurposeStrings>();
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
