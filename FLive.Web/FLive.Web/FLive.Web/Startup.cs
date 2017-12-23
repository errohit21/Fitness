using FLive.Web.Data;
using FLive.Web.Filters;
using FLive.Web.Models;
using FLive.Web.Repositories;
using FLive.Web.Services;
using FLive.Web.Services.Interfaces;
using FLive.Web.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Serilog;
using Serilog.Sinks.Slack;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;

namespace FLive.Web
{
    public partial class Startup
    {

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();

            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Warning()
                .Enrich.FromLogContext()
                .WriteTo.Slack("https://hooks.slack.com/services/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")
                .CreateLogger();
            //var mvcBuilder = serviceProvider.GetService<IMvcBuilder>();
            //new MvcConfiguration().ConfigureMvc(mvcBuilder);
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new SignalRContractResolver();


            var serializer = JsonSerializer.Create(settings);

            services.Add(new ServiceDescriptor(typeof (JsonSerializer),
                provider => serializer,
                ServiceLifetime.Transient));

            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddSignalR(options => { options.Hubs.EnableDetailedErrors = true; });

            
            services.AddMvc(options => { options.Filters.Add(new ApiExceptionFilter()); });
            //services.AddMvc(options =>
            //{
            //    options.SslPort = 44321;
            //    options.Filters.Add(new RequireHttpsAttribute());
            //});

            //var pathToDoc = Configuration["Swagger:Path"];

            //services.AddSwaggerGen();
            //services.ConfigureSwaggerGen(options =>
            //{
            //    options.SingleApiVersion(new Info
            //    {
            //        Version = "v1",
            //        Title = "Geo Search API",
            //        Description = "A simple api to search using geo location in Elasticsearch",
            //        TermsOfService = "None"
            //    });
            //    options.IncludeXmlComments(pathToDoc);
            //    options.DescribeAllEnumsAsStrings();
            //});

            services.AddOptions();
            services.Configure<WowzaConfig>(Configuration.GetSection("WowzaConfig"));
            services.Configure<ServiceBusConfig>(Configuration.GetSection("ServiceBusConfig"));
            services.Configure<StripeConfig>(Configuration.GetSection("StripeConfig"));
			services.Configure<SmsSettings>(Configuration.GetSection("SmsSettings"));
			services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));


			services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ISeedDataService, SeedDataService>();
            services.AddTransient<ISmsSender, SmsSender>();
            services.AddScoped<IStreamManagementService, StreamManagementService>();
            services.AddScoped<IMasterDataService, MasterDataService>();
            services.AddScoped<ILiveWorkoutService, LiveWorkoutService>();
            services.AddScoped<ITrainerService, TrainerService>();
            services.AddScoped<ISubscriberService, SubscriberService>();
            services.AddScoped<IPaymentGatewayServie, PaymentGatewayServie>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IAuthHelperService, AuthHelperService>();

			services.AddScoped<IAzurePushNotificationService, AzurePushNotifications>();


            //Repositories
            services.AddScoped<IFileUploadRepository, FileUploadRepository>();
            services.AddScoped(typeof (IGenericRepository<>), typeof (GenericRepository<>));

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "API", Version = "v1", TermsOfService = "None" });
                //c.DocInclusionPredicate((docName, apiDesc) =>
                //{
                //    if (apiDesc.HttpMethod == null) return false;
                //    return true;
                //});
               
            });

           

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Add Serilog to the logging pipeline
            loggerFactory.AddSerilog();

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();

                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
      .CreateScope())
                {
                    using (var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>())
                    {
                        context.Database.EnsureCreated();
                    }
                }
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();
            ConfigureAuth(app);
            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
               name: "Handoff-iOS9",
               template: ".well-known/apple-app-site-association",
               defaults: new { controller = "Home", action = "WellKnownAppleHandoff" });

                routes.MapRoute(
                    name: "ApplePay-MacOS",
                    template: ".well-known/apple-developer-merchantid-domain-association",
                    defaults: new { controller = "Home", action = "WellKnownApplePay" });


                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
           // routes.MapRoute("DefaultApi", "api/{controller}/{id}");
           

                //routes.MapRoute("default_route", "{controller}/{action}/{id?}");

            });
            
            //routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapHttpRoute(
            //    name: "swagger_root",
            //    routeTemplate: "",
            //    defaults: null,
            //    constraints: null,
            //    handler: new RedirectHandler((message => message.RequestUri.ToString()), "swagger"));

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);

            app.UseFacebookAuthentication(new FacebookOptions
            {
                AppId = "1357160437648964",
                AppSecret = "fa14b8fb9c891667e1a15aacd813f976"
            });

            app.ApplicationServices.GetService<ISeedDataService>().SeedData().Wait();

            //app.UseSwagger();
            //app.UseSwaggerUi();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
           

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
            });

            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);
            });

           

            app.UseSwagger();
          
            app.UseWebSockets();
            app.UseSignalR();
        }
    }
}