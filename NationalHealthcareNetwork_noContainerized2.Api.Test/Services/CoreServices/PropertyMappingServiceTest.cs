using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Organizations.Api.Models;
using Organizations.Api.Persistence.Entities;
using Organizations.Api.Services;
using Xunit;

namespace NationalHealthcareNetwork_noContainerized2.Api.Test.Services.CoreServices
{
    public class PropertyMappingServiceTest
    {
        [Fact]
        public void GetPropertyMapping_ThreePropertyMappings_Returns3()
        {
            var propertyMappingService = new PropertyMappingService();
            var organizationPropertyMapping =
                propertyMappingService.GetPropertyMapping<OrganizationDto, Organization>();
            Assert.Equal(3, organizationPropertyMapping.Count);
        }

        [Fact]
        public void GetPropertyMapping_ThreePropertyMappinga_ReturnsTheExpectedOnes()
        {
            var propertyMappingService = new PropertyMappingService();
            var organizationPropertyMapping =
                propertyMappingService.GetPropertyMapping<OrganizationDto, Organization>();
            Assert.Contains(organizationPropertyMapping["Id"].DestinationProperties, p =>p=="Id");
            Assert.Contains(organizationPropertyMapping["Name"].DestinationProperties, p =>p== "Name");
            Assert.Contains(organizationPropertyMapping["Description"].DestinationProperties, p =>p== "Description");
        }
    }
}
