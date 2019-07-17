using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Schaad.Accounting.Datasets;
using Schaad.Accounting.Interfaces;
using Schaad.Accounting.Repositories;
using Schaad.Accounting.Services;
using Schaad.Finance.Api;
using Schaad.Finance.Services;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using SimpleInjector.Lifestyles;

namespace Schaad.Accounting
{
    public class Startup
    {
        private readonly Container container = new Container();

        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            // Add simple injector
            services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(container));
            services.AddSingleton<IViewComponentActivator>(new SimpleInjectorViewComponentActivator(container));
            services.UseSimpleInjectorAspNetRequestScoping(container);

            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();
            services.AddSession(
                options =>
                {
                    options.IdleTimeout = TimeSpan.FromSeconds(3600);
                    options.Cookie.HttpOnly = true;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            InitializeContainer(app);

            app.UseSession();

            app.UseMvc(
                routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}");
                });
        }

        private void InitializeContainer(IApplicationBuilder app)
        {
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            // Add application presentation components:
            container.RegisterMvcControllers(app);
            container.RegisterMvcViewComponents(app);

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

            // Cross-wire ASP.NET services (if any):
            container.RegisterInstance(app.ApplicationServices.GetService<ILoggerFactory>());

            // The following registers a Func<T> delegate that can be injected as singleton,
            // and on invocation resolves a MVC IViewBufferScope service for that request.
            //container.RegisterSingleton<Func<IViewBufferScope>>(
            //    () => app.GetRequestService<IViewBufferScope>());

            try
            {
                container.Verify();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
    }
}