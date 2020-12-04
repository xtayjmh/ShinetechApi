using API.Auth;
using API.Controllers;
using API.Core;
using API.Filters;
using API.Interfaces;
using API.Models;
using API.Services;
using Autofac;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Shinetech.Infrastructure.Contract;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace API
{
    public class Startup
    {
        private const string _defaultCorsPolicyName = "any";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // public static readonly LoggerFactory MyLoggerFactory = new LoggerFactory(new[]
        // {
        //     new DebugLoggerProvider()
        // });

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // add cors config
            services.AddCors(options =>
            {
                options.AddPolicy(_defaultCorsPolicyName, builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                );
            });


            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());
            });
            string connecttext = Configuration.GetConnectionString("GeneralDbContext");
            //services.AddDbContext<EnsuranceDbContext>(options => options.UseSqlite(connecttext));
            services.AddDbContext<GeneralDbContext>(options => options
             .UseLoggerFactory(
                    LoggerFactory.Create(
                        logging => logging
                            .AddConsole()
                            .AddFilter(level => level >= LogLevel.Information)))
                .UseMySql(connecttext, new MySqlServerVersion(new Version(8, 0, 20)), mySqlOptions =>
                     mySqlOptions.EnableRetryOnFailure()
                ), ServiceLifetime.Transient);
            services.AddTransient<DbContext>(provider => provider.GetService<GeneralDbContext>());

            // register customer  filter
            services.AddControllers(options =>
            {
                // options.ReturnHttpNotAcceptable = true;
                options.Filters.Add(typeof(OTDActionFilter)); // Objet To DataModel validate
                options.Filters.Add(typeof(CustomExceptionFilterAttribute)); // Global exception action
                options.Filters.Add(typeof(WebApiResultFilter)); // Global Result action

                options.Conventions.Add(new GenericControllerRouteConvention());
                options.ModelMetadataDetailsProviders.Add(new RequiredBindingMetadataProvider());
            }
                ).ConfigureApplicationPartManager(m =>
                    m.FeatureProviders.Add(new GenericTypeControllerFeatureProvider()));

            // Swagger config
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API Document",
                    Version = "v1",
                    Description = "",
                });
                c.AddSecurityDefinition("Bearer", //Name the security scheme
                    new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme.",
                        Type = SecuritySchemeType
                            .Http, //We set the scheme type to http since we're using bearer authentication
                        Scheme =
                            "bearer" //The name of the HTTP Authorization scheme to be used in the Authorization header. In this case "bearer".
                    });

                c.OperationFilter<AddAuthTokenHeaderParameter>();
                //c.TagActionsBy(api => api.ActionDescriptor.RouteValues["controller"]);
                c.OrderActionsBy((apiDesc) => $"{apiDesc.HttpMethod}");
                // 为 Swagger JSON and UI设置xml文档注释路径
                var basePath =
                    Path.GetDirectoryName(typeof(Program).Assembly.Location); //获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                //c.IncludeXmlComments(Path.Combine(basePath ?? string.Empty, "Ensurance.API.xml"),
                //    includeControllerXmlComments: true);
                //c.IncludeXmlComments(Path.Combine(basePath ?? string.Empty, "Ensurance.Model.xml"),
                //    includeControllerXmlComments: true);
                // c.IncludeXmlComments(Path.Combine(basePath ?? string.Empty, "Ensurance.Infrastructure.Contract.xml"), includeControllerXmlComments: true);
                c.EnableAnnotations();
            });

            services.AddHttpContextAccessor();
            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
            // add auth
            services.AddAutoMapper(Assembly.GetAssembly(typeof(MapProfile)));
            services.AddAuthServices(Configuration);
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(_defaultCorsPolicyName);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();


            app.UseRouting();

            app.UseAuthorization();

            app.UseStaticFiles();
            var filesFolder = Path.Combine(Directory.GetCurrentDirectory(), "files");
            if (!Directory.Exists(filesFolder))
            {
                Directory.CreateDirectory(filesFolder);
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(filesFolder),
                RequestPath = "/files"
            });

            app.UseCookiePolicy();

            app.UseWebSockets();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<Notifications>("/notifications");
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Document");
                c.RoutePrefix = string.Empty;
                c.EnableFilter();
            });
            app.UseAuthentication();
        }

        public virtual void ConfigureContainer(ContainerBuilder builder)
        {

            builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(API.Services.AuthService)))
               .Where(t => t.IsAssignableTo<API.Interfaces.IService>())
               .AsImplementedInterfaces()
               .InstancePerDependency();
            builder.RegisterGeneric(typeof(CrudService<,,>)).As(typeof(ICrudService<,,>)).InstancePerDependency();



            builder.RegisterType<EFUnitOfWork>()
                .As<IUnitOfWork>();
            builder.RegisterGeneric(typeof(CrudRepository<>)).As(typeof(ICrudRepository<>)).InstancePerDependency();
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerDependency();


            //var config = new ConfigurationBuilder()
            //    .AddJsonFile("autofac.json", optional: true)
            //    .Build();

            //var configModule = new ConfigurationModule(config);
            //builder.RegisterModule(configModule);
        }
    }

    /// <summary>
    /// 处理返回的时间格式带T
    /// </summary>
    public class DatetimeJsonConverter : System.Text.Json.Serialization.JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (DateTime.TryParse(reader.GetString(), out DateTime date))
                {
                    return date;
                }
            }
            return reader.GetDateTime();
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
        }

    }
}
