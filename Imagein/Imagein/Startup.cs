using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Imagein.Data.DbContexts;
using Imagein.Data.Repositories;
using Imagein.Data.Repositories.Base;
using Imagein.Data.Repositories.Interface;
using Imagein.Services;
using Imagein.Services.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Imagein
{
    public class Startup
    {

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Env { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            this.Configuration = configuration;
            this.Env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // PG config
            var sqlConnectionString = Configuration.GetConnectionString("DefaultConnection");
            var migrationAssembly = typeof(ApplicationDbContext).Assembly.GetName().Name;
            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseNpgsql(sqlConnectionString, b => b.MigrationsAssembly(migrationAssembly));
            });


            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IFileRepository, FileRepository>();
            services.AddTransient<IFileService, FileService>();

            services
                .AddMvc()
                .AddJsonOptions(options => {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                    options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include; // serializing only
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }

            app.UseMvc();
        }
    }
}
