using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Skinet.Infrastructure.Data;
using Skinet.WebApi.Extensions;
using Skinet.WebApi.Helpers;
using Skinet.WebApi.Middleware;

namespace Skinet.WebApi
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
            services.AddControllers();

            services.AddDbContext<StoreContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddAutoMapper(typeof(MappingProfiles));

            services.AddApplicationServices();
            services.AddSwaggerDocumentation();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins("https://localhost:4200");
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseStaticFiles();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            app.UseSwaggerDocumentation();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
