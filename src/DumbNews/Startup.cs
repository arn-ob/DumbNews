using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DumbNews.Lib.Filters;
using DumbNews.Lib.Options;
using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Reflection;
using DumbNews.Lib.Utility;

namespace DumbNews
{
    public class Startup
    {
        private ILoggerFactory loggerFactory;

        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            InitiateDbConnection();
            ConfigureDb();
        }

        private void InitiateDbConnection()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Configuration.GetSection("ConnectionStrings")["StorageConnection"]);
            TableClient = storageAccount.CreateCloudTableClient();
        }

        public IConfigurationRoot Configuration { get; set; }
        public CloudTableClient TableClient { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc().AddMvcOptions(o =>
            {
                o.Filters.Add(new ExceptionFilter(this.loggerFactory));
            });

            services.AddOptions();
            services.Configure<ConnectionStringsSettings>(Configuration.GetSection("ConnectionStrings"));

            services.AddSingleton((svc) => this.TableClient);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIISPlatformHandler();

            app.UseStaticFiles();

            app.UseMvc();


        }

        private void ConfigureDb()
        {
            var entities = ReflectionUtils.GetTypesInNamespace(typeof(Startup).GetTypeInfo().Assembly, "DumbNews.Lib.Entity");
            foreach (var entity in entities)
            {
                InstantiateTable(entity, TableClient);
            }
        }

        private void InstantiateTable(Type entity, CloudTableClient tableClient)
        {
            var table = tableClient.GetTableReference(string.Concat(entity.Name, "s"));
            var Task = table.CreateIfNotExistsAsync();
            Task.Wait();
        }


        // Entry point for the application.
        public static void Main(string[] args)
        {

            WebApplication.Run<Startup>(args);
        }
    }
}
