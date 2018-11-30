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
using QuizRT.Settings;
using Microsoft.AspNetCore.Hosting.Internal;
using Consul;
using Swashbuckle.AspNetCore.Swagger;

namespace QuizRTapi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env; // For Mongo Config Env Variable
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }  // For Mongo Config Env Variable

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddScoped<QuizRTContext>();
            
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });

            // Config For Service Discovery - Consul
            // services.Configure<ConsulConfig>(Configuration.GetSection("consulConfig"));
            // services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig => {
            //     var address = Configuration["consulConfig:address"];
            //     consulConfig.Address = new Uri(address);
            // }));

            // For MongoDb Connection String
            services.Configure<MongoDBSettings>( options => {
                Console.WriteLine("---------MongoDBSettings----------");
                options.ConnectionString = Configuration.GetSection("MongoDb:ConnectionString").Value;
                options.Database = Configuration.GetSection("MongoDb:Database").Value;
                options.Container = Configuration.GetSection("MongoDb:Container").Value;
                options.IsContained  = Configuration["DOTNET_RUNNING_IN_CONTAINER"] != null;
                options.Development = HostingEnvironment.IsDevelopment();   // For Mongo Config Env Variable
                Console.WriteLine("---------"+options.ConnectionString+"---------");
                Console.WriteLine("---------"+options.Database+"---------");
                Console.WriteLine("---------"+options.Container+"---------");
                Console.WriteLine("---------"+options.IsContained+"---------");
                Console.WriteLine("---------"+options.Development+"---------");
            });
            services.AddScoped<IGameContext, QuizRTContext>();

            // Database connection string for MsSQL Express.
            // Make sure to update the Password value below from "Your_password123" to your actual password.
            // var connection = @"Server=db;Database=master;User=sa;Password=Your_password123;";
            // This line uses 'UseSqlServer' in the 'options' parameter
            // with the connection string defined above.
            // services.AddDbContext<QuizRTContext>(options => options.UseSqlServer(connection));

            // var connString = Environment.GetEnvironmentVariable("SQLSERVER_HOST") ?? "Server=localhost\\SQLEXPRESS;Database=QuiztestDB;Trusted_Connection=True;";
            // var password = Environment.GetEnvironmentVariable("SQLSERVER_SA_PASSWORD") ?? "Testing123";
            // var connString = $"Data Source={hostname};Initial Catalog=KontenaAspnetCore;User ID=sa;Password={password};";
            // services.AddDbContext<QuizRTContext>(options => options.UseSqlServer(connString));

            // var hostname = Environment.GetEnvironmentVariable("SQLSERVER_HOST");
            // if(string.IsNullOrEmpty(hostname)){
            //     hostname = Configuration.GetConnectionString("QuizRTTest");
            // }
            // services.AddDbContext<QuizRTContext>(options => options.UseSqlServer(hostname));

            // services.AddDbContext<QuizRTContext>(); // Before Docker
            // services.AddScoped<IQuizRTRepo,QuizRTRepo>();
            
            services.AddScoped<IQuizRTRepo,QuizRTRepo>();
            services.AddCors(); // adding CORS service for use in  Configure fn *hellokuldeep
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
                                // ILoggerFactory loggerFactory, IApplicationLifetime lifetime)
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

            // loggerFactory.AddConsole();

            // app.UseHttpsRedirection();
            app.UseMvc();
            // app.RegisterWithConsul(lifetime);

        }
    }
}
