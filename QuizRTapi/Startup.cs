using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using QuizRT.Models;

namespace QuizRTapi
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddScoped<IQuizRTRepo,QuizRTRepo>();

            // Database connection string.
            // Make sure to update the Password value below from "Your_password123" to your actual password.
            // var connection = @"Server=db;Database=master;User=sa;Password=Your_password123;";
            // This line uses 'UseSqlServer' in the 'options' parameter
            // with the connection string defined above.
            // services.AddDbContext<QuizRTContext>(options => options.UseSqlServer(connection));

            var connString = Environment.GetEnvironmentVariable("SQLSERVER_HOST") ?? "Server=localhost\\SQLEXPRESS;Database=QuiztestDB;Trusted_Connection=True;";
            // var password = Environment.GetEnvironmentVariable("SQLSERVER_SA_PASSWORD") ?? "Testing123";
            // var connString = $"Data Source={hostname};Initial Catalog=KontenaAspnetCore;User ID=sa;Password={password};";
            services.AddDbContext<QuizRTContext>(options => options.UseSqlServer(connString));

            // var hostname = Environment.GetEnvironmentVariable("SQLSERVER_HOST");
            // if(string.IsNullOrEmpty(hostname)){
            //     hostname = Configuration.GetConnectionString("QuizRTTest");
            // }
            // services.AddDbContext<QuizRTContext>(options => options.UseSqlServer(hostname));

            // services.AddDbContext<QuizRTContext>(); // Before Docker
            services.AddCors(); // adding CORS service for use in  Configure fn *hellokuldeep
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(builder =>
                builder//.WithOrigins("http://localhost:4200")
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
            ); //for CORS *hellokuldeep

            // app.UseHttpsRedirection();
            app.UseMvc();

        }
    }
}
