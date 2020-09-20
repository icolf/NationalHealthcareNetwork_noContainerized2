using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NationalHealthcareNetwork_noContainerized2.Api.Test.Helpers;
using NationalHealthcareNetwork_noContainerized2.Api.Test.Services.Logger;
using Organizations.Api.Helpers;
using Organizations.Api.Models;
using Organizations.Api.Persistence;
using Organizations.Api.Persistence.Entities;
using Organizations.Api.Services;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace NationalHealthcareNetwork_noContainerized2.Api.Test.Extensions
{
    public class IQueryableExtensionsTest
    {
        private readonly ITestOutputHelper _output;
        private readonly IPropertyMappingService _propertyMappingService;

        public IQueryableExtensionsTest(ITestOutputHelper output)
        {
            _output = output;
            _propertyMappingService = new PropertyMappingService();
        }

        [Fact]
        public void ApplySort_NullPropertyMapping_ReturnsNullException()
        {
            IQueryable<Organization> nullQueryable=Enumerable.Empty<Organization>().AsQueryable(); 
            var organizationResourceParameters = new OrganizationResourceParameters { OrderBy = "Name desc" };

            Assert.Throws<ArgumentNullException>(() => 
                nullQueryable.ApplySort(organizationResourceParameters.OrderBy, 
                    null));
        }
        [Fact]
        public void ApplySort_EmptyOrderBy_ReturnsNullException()
        {
            IQueryable<Organization> nullQueryable=Enumerable.Empty<Organization>().AsQueryable(); 
            var organizationResourceParameters = new OrganizationResourceParameters { OrderBy = "" };

            Assert.Equal(nullQueryable,
                nullQueryable.ApplySort(organizationResourceParameters.OrderBy,
                    _propertyMappingService.GetPropertyMapping<OrganizationDto, Organization>()));
        }

        [Fact]
        public void ApplySort_3Organizations_ReturnInExpectedOrder()
        {
            //Arrange

            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };

            var connection = new SqliteConnection(connectionStringBuilder.ToString());

            var options = new DbContextOptionsBuilder<OrganizationsContext>()
                .UseLoggerFactory(new LoggerFactory(
                    new[] { new LogToActionLoggerProvider((log => { _output.WriteLine(log); })) }))
                .UseSqlite(connection)
                .Options;
            var mappingConfig = TestHelpers.SetMapper();

            IMapper mapper = mappingConfig.CreateMapper();


            using (var context = new OrganizationsContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();
                Helpers.TestHelpers.AddFiveOrganizations(context);

                var organizationResourceParameters = new OrganizationResourceParameters {OrderBy = "Name desc" };
                Assert.Equal("Organization1", context.Organizations.First().Name);

                var orderedOrganizations = context.Organizations.ApplySort(organizationResourceParameters.OrderBy,
                    _propertyMappingService.GetPropertyMapping<OrganizationDto, Organization>());

                //Assert
                Assert.Equal("Organization2", orderedOrganizations.First().Name);
                Assert.Equal("Org1", orderedOrganizations.Last().Name);
            }


        }
    }
}
