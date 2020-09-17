using System.Linq;
using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NationalHealthcareNetwork_noContainerized2.Api.Test.Helpers;
using NationalHealthcareNetwork_noContainerized2.Api.Test.Services.Logger;
using Organizations.Api.Helpers;
using Organizations.Api.Persistence;
using Organizations.Api.Repositories;
using Organizations.Api.Services;
using Xunit;
using Xunit.Abstractions;

namespace NationalHealthcareNetwork_noContainerized2.Api.Test.Repositories
{
    public class OrganizationRepositoryTest
    {
        private readonly ITestOutputHelper _output;

        public OrganizationRepositoryTest(ITestOutputHelper output)
        {
            _output = output;
        }
        [Fact]
        public void GetOrganizationsOnly_7OrganizationsInDB_Returns7Organizations()
        {
            //Arrange

            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };

            var connection = new SqliteConnection(connectionStringBuilder.ToString());

            var options = new DbContextOptionsBuilder<OrganizationsContext>()
                .UseLoggerFactory(new LoggerFactory(
                    new [] {new LogToActionLoggerProvider((log => { _output.WriteLine(log); })) }))
                .UseSqlite(connection)
                .Options;
            
            //This code is for setting InMemory Database
            //var options = new DbContextOptionsBuilder<OrganizationsContext>()
            //    .UseInMemoryDatabase($"OrganizationInMemoryDBForTesting{Guid.NewGuid()}")
            //    .Options;

            var mappingConfig = TestHelpers.SetMapper();

            IMapper mapper = mappingConfig.CreateMapper();


            using (var context = new OrganizationsContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();

                Helpers.TestHelpers.AddFiveOrganizations(context);
            }

            var organizationResourceParameters = new OrganizationResourceParameters()
            {
                PageSize = 5,
                CurrentPage = 1
            };

            using (var context = new OrganizationsContext(options))
            {
                //Act
                var organizationRepository = new OrganizationsRepository(context, mapper, new PropertyMappingService());
                var organizations = organizationRepository.GetOrganizationsOnly();
                //Assert
                Assert.Equal(7, organizations.Count());
            }
        }

        [Fact]
        public void GetOrganizations_PageSizeIsThreeAndCurrentPageIsTwo_ReturnThreeOrganizations()
        {
            //Arrange

            var connectionStringBuilder = new SqliteConnectionStringBuilder {DataSource = ":memory:"};
            var connection = new SqliteConnection(connectionStringBuilder.ToString());
            var options = new DbContextOptionsBuilder<OrganizationsContext>().UseSqlite(connection).Options;
            
            //var options = new DbContextOptionsBuilder<OrganizationsContext>()
            //    .UseInMemoryDatabase($"OrganizationInMemoryDBForTesting{Guid.NewGuid()}")
            //    .Options;

            var mappingConfig = TestHelpers.SetMapper();

            IMapper mapper = mappingConfig.CreateMapper();


            using (var context = new OrganizationsContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();

                Helpers.TestHelpers.AddFiveOrganizations(context);
            }

            var organizationResourceParameters = new OrganizationResourceParameters()
            {
                PageSize = 3,
                CurrentPage = 1
            };

            using (var context = new OrganizationsContext(options))
            {
                var organizationRepository = new OrganizationsRepository(context, mapper, new PropertyMappingService());
                var organizations = organizationRepository.GetOrganizations(organizationResourceParameters).Result;
                
                
                //Assert
                Assert.Equal(3, organizations.TotalPages);
                Assert.Equal(7, organizations.TotalCount);

            }

        }
    }
}
