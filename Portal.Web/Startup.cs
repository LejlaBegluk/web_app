using AspNetCore.Reporting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Portal.Data;
using Portal.Data.Entities;
using Portal.Data.Helpers;
using Portal.Data.Helpers.HelpersInterfaces;
using Portal.Persistance;
using Portal.Persistance.Repositories.Interfaces;
using Portal.Web.Extensions;
using Portal.Web.Services;
using Wangkanai.Detection;

namespace Portal.Web
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            //services.AddScoped(opt=>new LocalReport(Configuration["ReportPath"]));
            services.AddScoped(opt => new LocalReport(Configuration["ReportPath"]));

            services.AddDbContext<PortalDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DevConnection")));
            services.AddIdentity<User, Role>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password = new PasswordOptions
                {
                    RequireDigit = true,
                    RequiredLength = 8,
                    RequireLowercase = true,
                    RequireUppercase = true,
                    RequireNonAlphanumeric = false

                };
            }).AddEntityFrameworkStores<PortalDbContext>().AddDefaultTokenProviders();
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            services.AddCors();
            services.AddDetection();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdministratorOnly", policy => policy.RequireRole("Administrator"));
                options.AddPolicy("EditorOnly", policy => policy.RequireRole("Editor"));
                options.AddPolicy("JournalistOnly", policy => policy.RequireRole("Journalist", "Administrator", "Editor"));
                options.AddPolicy("ArticleOwnerOnly", policy => policy.Requirements.Add(new SameArticleRequirement()));
                options.AddPolicy("AccountOwnerOnly", policy => policy.Requirements.Add(new SamePortalUserRequirement()));

            });
            services.AddTransient<PortalDataSeed>();
            services.AddTransient<IUserFilterOrderHelper, UserFilterOrderHelper>();
            services.AddTransient<IArticleFilterOrderHelper, ArticleFilterOrderHelper>();
            services.AddTransient<ICategoryFilterOrderHelper, CategoryFilterOrderHelper>();
            services.AddTransient<IPollFilterOrderHelper, PollFilterOrderHelper>();
            services.AddTransient<ICorrespondentArticleFilterOrderHelper, CorrespondentArticleFilterOrderHelper>();

            services.AddTransient<IMailService, MailService>();
            services.AddTransient<IPhotoService, PhotoService>();
            services.AddSingleton<IAuthorizationHandler, ArticleAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, UserAuthorizationHandler>();
          


        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, PortalDataSeed seed)
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
            app.ExecuteMigrations<PortalDbContext>();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            if (env.IsDevelopment())
                seed.SeedTestData();
        }
    }
}
