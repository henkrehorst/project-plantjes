using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using project_c.Services;
using User = project_c.Models.Users.User;

[assembly: HostingStartup(typeof(project_c.Areas.Identity.IdentityHostingStartup))]
namespace project_c.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<DataContext>(options =>
                    options.UseNpgsql(context.Configuration.GetConnectionString("PlantjesDataContext"),
                        o => o.UseNetTopologySuite()));

                services.AddDefaultIdentity<User>(options =>
                    {
                        options.SignIn.RequireConfirmedEmail = true;
                        options.SignIn.RequireConfirmedAccount = true;
                        options.User.AllowedUserNameCharacters = string.Empty;
                    })
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<DataContext>();
               
            });
        }
    }
}