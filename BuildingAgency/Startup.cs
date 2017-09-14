using BuildingAgency.DataAccessLevel;
using BuildingAgency.Models;
using BuildingAgency.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BuildingAgency
{
    //public class AuthorEntityBinder : IModelBinder
    //{
    //    private readonly AppDbContext _db;
    //    public AuthorEntityBinder(AppDbContext db)
    //    {
    //        _db = db;
    //    }

    //    public Task BindModelAsync(ModelBindingContext bindingContext)
    //    {
    //        if (bindingContext == null)
    //        {
    //            throw new ArgumentNullException(nameof(bindingContext));
    //        }

    //        // Specify a default argument name if none is set by ModelBinderAttribute
    //        var modelName = bindingContext.BinderModelName;
    //        if (string.IsNullOrEmpty(modelName))
    //        {
    //            modelName = "authorId";
    //        }

    //        // Try to fetch the value of the argument by name
    //        var valueProviderResult =
    //            bindingContext.ValueProvider.GetValue(modelName);

    //        if (valueProviderResult == ValueProviderResult.None)
    //        {
    //            return TaskCache.CompletedTask;
    //        }

    //        bindingContext.ModelState.SetModelValue(modelName,
    //            valueProviderResult);

    //        var value = valueProviderResult.FirstValue;

    //        // Check if the argument value is null or empty
    //        if (string.IsNullOrEmpty(value))
    //        {
    //            return TaskCache.CompletedTask;
    //        }

    //        int id = 0;
    //        if (!int.TryParse(value, out id))
    //        {
    //            // Non-integer arguments result in model state errors
    //            bindingContext.ModelState.TryAddModelError(
    //                                    bindingContext.ModelName,
    //                                    "Author Id must be an integer.");
    //            return TaskCache.CompletedTask;
    //        }

    //        // Model will be null if not found, including for 
    //        // out of range id values (0, -3, etc.)
    //        var model = _db.Authors.Find(id);
    //        bindingContext.Result = ModelBindingResult.Success(model);
    //        return TaskCache.CompletedTask;
    //    }
    //}
    //public class EFModelBinderProvider : IModelBinderProvider
    //{
    //    public IModelBinder GetBinder(ModelBinderProviderContext context)
    //    {
    //        if (context.GetType() == typeof(double))
    //        {
    //            return new DoubleModelBinder();
    //        }
    //        return null;
    //    }
    //}

    //public class DoubleModelBinder : IModelBinder
    //{
    //    public Task BindModelAsync(ModelBindingContext bindingContext)
    //    {
    //        var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
    //        //var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

    //        //var modelState = new ModelState { Value = valueResult };
    //        object actualValue = null;
    //        try
    //        {
    //            actualValue = Convert.ToDouble(valueResult.FirstValue, CultureInfo.InvariantCulture);
    //        }
    //        catch (FormatException e)
    //        {
    //            //modelState.Errors.Add(e);
    //            bindingContext.ModelState.TryAddModelError(
    //                                    bindingContext.ModelName,
    //                                    "test lol");
    //        }

    //        //bindingContext.ModelState.Add(bindingContext.ModelName, bindingContext.ModelState);
    //        return TaskCache.CompletedTask;
    //    }
    //}

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            //services.AddMvc(options =>
            //{
            //    // add custom binder to beginning of collection
            //    options.ModelBinderProviders.Insert(0, new EFModelBinderProvider());
            //});

            services.AddScoped<StaffsService>();
            services.AddScoped<AccountService>();
            services.AddScoped<ClientsService>();
            services.AddScoped<ViewingsService>();
            services.AddScoped<ContractsService>();
            services.AddScoped<PrivateOwnersService>();
            services.AddScoped<PropertyForRentsService>();

            services.AddScoped<StaffsRepository>();
            services.AddScoped<AccountRepository>();
            services.AddScoped<ClientsRepository>();
            services.AddScoped<ViewingsRepository>();
            services.AddScoped<ContractsRepository>();
            services.AddScoped<PrivateOwnersRepository>();
            services.AddScoped<PropertyForRentsRepository>();

            services.AddDbContext<BuildingAgencyContext>(options =>
                     options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")
                     ));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("en-US"),
                new CultureInfo("es"),
                new CultureInfo("es-ES")
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseStaticFiles();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "Cookies",
                LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //app.UseStatusCodePagesWithRedirects("~/errors/{0}");

            // инициализация базы данных
            DatabaseInitialize(app.ApplicationServices);
        }

        public void DatabaseInitialize(IServiceProvider serviceProvider)
        {
            string adminRoleName = "admin";
            string userRoleName = "user";

            string adminEmail = "admin@mail.ru";
            string adminPassport = "AM123456";
            string adminPassword = CalculateMD5Hash("123");

            using (BuildingAgencyContext db = serviceProvider.GetRequiredService<BuildingAgencyContext>())
            {
                Role adminRole = db.Role.FirstOrDefault(x => x.Name == adminRoleName);
                Role userRole = db.Role.FirstOrDefault(x => x.Name == userRoleName);

                // добавляем роли, если их нет
                if (adminRole == null)
                {
                    adminRole = new Role { Name = adminRoleName };
                    db.Role.Add(adminRole);
                }
                if (userRole == null)
                {
                    userRole = new Role { Name = userRoleName };
                    db.Role.Add(userRole);
                }
                db.SaveChanges();

                // добавляем администратора, если его нет
                User admin = db.User.FirstOrDefault(u => u.Email == adminEmail);
                if (admin == null)
                {
                    db.User.Add(new User { Email = adminEmail, Password = adminPassword, Passport = adminPassport, Role = adminRole });
                    db.SaveChanges();
                }
            }
        }

        public static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();

            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
