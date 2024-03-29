using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using project_c.Repository;
using project_c.Services;
using project_c.Services.GeoRegister.Client;
using project_c.Services.GeoRegister.Handler;
using project_c.Services.GeoRegister.Service;
using project_c.Hubs;

namespace project_c
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
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddDbContext<DataContext>(builder =>
                
                builder.UseNpgsql(Configuration.GetConnectionString("PlantjesDataContext"), 
                    o => o.UseNetTopologySuite()));
            services.AddRazorPages();
            services.AddTransient<UploadService>();
            services.AddTransient<ZipCodeService>();
            services.AddTransient<ZipCodeHandler>();
            services.AddTransient<HttpClient>();
            services.AddTransient<PlantRepository>();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [System.Obsolete]
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

            if (Configuration.GetConnectionString("Environment") == "Prod")
            {
                UpdateDatabase(app);
            }
            else
            {
                app.UseHttpsRedirection();
            }
            
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseSignalR(route =>
            {
                route.MapHub<ChatHub>("/api/chat");
            });

            app.UseEndpoints(endpoints =>
            {
                
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
        
        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<DataContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}