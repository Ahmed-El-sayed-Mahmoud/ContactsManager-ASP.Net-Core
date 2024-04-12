using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Entities;

namespace TestProject
{
    public class CustomWebAppFactory :WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {

            base.ConfigureWebHost(builder);
            builder.UseEnvironment("Test");
            builder.ConfigureServices(services =>
            {

                ServiceDescriptor? serviceDescriptor = services.SingleOrDefault(t => t.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (serviceDescriptor != null)
                {
                    services.Remove(serviceDescriptor);
                }
                services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("TestDbB"));
            });
        }
    }
}
