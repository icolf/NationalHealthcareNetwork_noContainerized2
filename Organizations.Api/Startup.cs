using System;
using System.IO;
using System.Reflection;
using System.Threading;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Organizations.Api.AutoMapperProfiles;
using Organizations.Api.Models;
using Organizations.Api.Models.CreationDtos;
using Organizations.Api.Models.UpdateDtos;
using Organizations.Api.Persistence;
using Organizations.Api.Persistence.Entities;
using Organizations.Api.Repositories;
using Organizations.Api.Repositories.RepositoriesInterfaces;
using Organizations.Api.Services;

namespace Organizations.Api
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;
        public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            _logger = logger;
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ThreadPool.SetMaxThreads(Environment.ProcessorCount, Environment.ProcessorCount);
            //services.AddMvc()
            //    .AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
            //    .AddJsonOptions(options =>
            //    {
            //        if (options.SerializerSettings.ContractResolver != null)
            //        {
            //            var castedResolver = options.SerializerSettings.ContractResolver as DefaultContractResolver;
            //            castedResolver.NamingStrategy = null;
            //        }
            //    })
            //    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(x => {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });


            services.AddMvc(setupAction =>
                {
                    setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
                    setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
                    setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
                    setupAction.ReturnHttpNotAcceptable = true;
                    setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                })
                .AddJsonOptions(jo =>
                {
                    jo.SerializerSettings.ContractResolver=new CamelCasePropertyNamesContractResolver();
                })
                .AddMvcOptions(o=>o.InputFormatters.Add(new XmlSerializerInputFormatter(o)))
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var organizationsDbConnectionString = Configuration["connectionStrings:organizationsDbConnectionString"];
            services.AddDbContext<OrganizationsContext>(o => o.UseSqlServer(organizationsDbConnectionString));

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var actionExecutingContext = actionContext as ActionExecutingContext;
                    //if there are modelstate errors & and all key values are correctly
                    //found/parsed we're dealing with validation errors
                    if (actionContext.ModelState.ErrorCount > 0 &&
                        actionExecutingContext?.ActionArguments.Count ==
                        actionContext.ActionDescriptor.Parameters.Count)
                    {
                        return new UnprocessableEntityObjectResult(actionContext.ModelState);
                    }

                    return new BadRequestObjectResult(actionContext.ModelState);
                };
            });


            services.AddScoped<IOrganizationsRepository, OrganizationsRepository>();
            services.AddScoped<IAddressesRepository, AddressesRepository>();
            services.AddScoped<IPhonesRepository, PhonesRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddTransient<ITypeHelperServices, TypeHelperServices>();
            _logger.LogInformation("Added repositories to services");

            //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<OrganizationsProfile>();

                cfg.CreateMap<Address, AddressDto>();
                cfg.CreateMap<Phone, PhoneDto>();
                cfg.CreateMap<PhoneForCreationDto, Phone>();
                cfg.CreateMap<AddressForCreationDto, Address>();
                cfg.CreateMap<AddressForUpdateDto, Address>();
                cfg.CreateMap<PhoneForUpdateDto, Phone>();
                cfg.CreateMap<Address, AddressForUpdateDto>();
                cfg.CreateMap<Phone, PhoneForUpdateDto>();

            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);


            //Adding swagger configuration
            services.AddSwaggerGen(setupAction=>
            {
                setupAction.SwaggerDoc("NationalHealthSpecification", new OpenApiInfo()
                {
                    Title="NationalHealthCareNetwork",
                    Version="1",
                    Description= "API as a sample for demonstrating my skills in ASP .Net Core",
                    Contact=new OpenApiContact
                    {
                        Email="icolfigueroa@gmail.com",
                        Name="Luis E. Figueroa",
                        Url= new Uri("https://www.linkedin.com/in/luis-e-figueroa-9a99a093/")
                    }
                });

                var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
                setupAction.IncludeXmlComments(xmlCommentsFullPath);
            });


            services.AddHttpCacheHeaders(expirationModelOptions =>
            {
                expirationModelOptions.MaxAge = 6;
            });

            services.AddResponseCaching();
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
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (exceptionHandlerFeature != null)
                        {
                            _logger.LogError(500, exceptionHandlerFeature.Error, exceptionHandlerFeature.Error.Message);
                        }
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened.  Try again later!");
                    });
                });
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStatusCodePages();

            app.UseResponseCaching();
            app.UseHttpCacheHeaders();
            app.UseSwagger();
            app.UseSwaggerUI(setupAction =>
            {
                setupAction.SwaggerEndpoint(
                    "/swagger/NationalHealthSpecification/swagger.json", 
                    "NationalHealthCareNetwork");
                setupAction.RoutePrefix = "";
            });
            
            app.UseMvc();
        }
    }
}
