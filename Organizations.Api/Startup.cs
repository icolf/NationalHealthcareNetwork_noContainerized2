using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using Organizations.Api.AutoMapperProfiles;
using Organizations.Api.Models;
using Organizations.Api.Models.CreationDtos;
using Organizations.Api.Models.UpdateDtos;
using Organizations.Api.Persistence;
using Organizations.Api.Persistence.Entities;
using Organizations.Api.Repositories;
using Organizations.Api.Repositories.RepositoriesInterfaces;

namespace Organizations.Api
{
    public class Startup
    {
        public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
                .AddJsonOptions(options =>
                {
                    if (options.SerializerSettings.ContractResolver != null)
                    {
                        var castedResolver = options.SerializerSettings.ContractResolver as DefaultContractResolver;
                        castedResolver.NamingStrategy = null;
                    }
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var organizationsDbConnectionString = Startup.Configuration["connectionStrings:organizationsDbConnectionString"];
            services.AddDbContext<OrganizationsContext>(o => o.UseSqlServer(organizationsDbConnectionString));

            services.AddScoped<IOrganizationsRepository, OrganizationsRepository>();
            services.AddScoped<IAddressesRepository, AddressesRepository>();
            services.AddScoped<IPhonesRepository, PhonesRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

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

            app.UseHttpsRedirection();
            app.UseStatusCodePages();



            app.UseMvc();
        }
    }
}
