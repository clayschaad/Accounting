using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleInjector;
using Schaad.Accounting.Datasets;
using Schaad.Accounting.Interfaces;
using Schaad.Accounting.Repositories;
using Schaad.Accounting.Services;
using Schaad.Finance.Api;
using Schaad.Finance.Services;
using System;
using Schaad.Accounting.Converter;

namespace Schaad.Accounting
{
    public class Startup
    {
        private Container container = new Container();

        public Startup(IConfiguration configuration)
        {
            // Set to false. This will be the default in v5.x and going forward.
            container.Options.ResolveUnregisteredConcreteTypes = false;

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // ASP.NET default stuff here
            services.AddControllersWithViews()
               .AddJsonOptions(options =>
               {
                   options.JsonSerializerOptions.Converters.Add(new DecimalConverter());
               });

            services.AddLogging();
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            // Sets up the basic configuration that for integrating Simple Injector with
            // ASP.NET Core by setting the DefaultScopedLifestyle, and setting up auto
            // cross wiring.
            services.AddSimpleInjector(container, options =>
            {
                // AddAspNetCore() wraps web requests in a Simple Injector scope and
                // allows request-scoped framework services to be resolved.
                options.AddAspNetCore()

                // Ensure activation of a specific framework type to be created by
                // Simple Injector instead of the built-in configuration system.
                // All calls are optional. You can enable what you need. For instance,
                // ViewComponents, PageModels, and TagHelpers are not needed when you
                // build a Web API.
                .AddControllerActivation()
                .AddViewComponentActivation()
                .AddPageModelActivation()
                .AddTagHelperActivation();

                // Optionally, allow application components to depend on the non-generic
                // ILogger (Microsoft.Extensions.Logging) or IStringLocalizer
                // (Microsoft.Extensions.Localization) abstractions.
                options.AddLogging();
                options.AddLocalization();
            });

            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();
            services.AddSession(
                options =>
                {
                    options.IdleTimeout = TimeSpan.FromSeconds(3600);
                    options.Cookie.HttpOnly = true;
                });

            InitializeContainer();
        }

        private void InitializeContainer()
        {
            var settings = Configuration.GetSection("Settings").Get<SettingsDataset>();
            container.Register<ISettingsService>(() => new SettingsService(settings), Lifestyle.Singleton);

            container.Register<IAccountRepository, AccountRepository>(Lifestyle.Scoped);
            container.Register<IBankTransactionRepository, BankTransactionRepository>(Lifestyle.Scoped);
            container.Register<IBookingRuleRepository, BookingRuleRepository>(Lifestyle.Scoped);
            container.Register<IBookingTextRepository, BookingTextRepository>(Lifestyle.Scoped);
            container.Register<ISplitPredefinitonRepository, SplitPredefinitonRepository>(Lifestyle.Scoped);
            container.Register<ISubclassRepository, SubclassRepository>(Lifestyle.Scoped);
            container.Register<ITransactionRepository, TransactionRepository>(Lifestyle.Scoped);

            container.Register<IChartService, ChartService>();
            container.Register<IViewService, ViewService>();
            container.Register<IFileService, FileService>();
            container.Register<IAccountStatementService, AccountStatementService>();
            container.Register<ICreditCardStatementService, CreditCardStatementService>();
            container.Register<IFxService, FxService>(Lifestyle.Singleton);
            container.Register<IPdfParsingService, PdfParsingService>(Lifestyle.Singleton);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // UseSimpleInjector() finalizes the integration process.
            app.UseSimpleInjector(container);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Default ASP.NET middleware
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseSession();

            // ASP.NET MVC default stuff here
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            // Always verify the container
            container.Verify();
        }
    }
}
