using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Organizations.Api.AutoMapperProfiles;
using Organizations.Api.Models;
using Organizations.Api.Models.CreationDtos;
using Organizations.Api.Models.UpdateDtos;
using Organizations.Api.Persistence;
using Organizations.Api.Persistence.Entities;

namespace NationalHealthcareNetwork_noContainerized2.Api.Test.Helpers
{
    public class TestHelpers
    {
        public static void AddFiveOrganizations(OrganizationsContext context)
        {
            var newGuid = Guid.NewGuid();
            context.Phones.Add(new Phone()
            {
                PhoneNumber = "(111) 111-1111",
                OrganizationId = newGuid
            });
            context.Addresses.Add(new Address()
            {
                Address1 = "Address1 Organization1",
                City = "Kansas City",
                State = "MO",
                Country = "USA",
                Zip = "64158",
                PostalAddress1 = "Address1 Organization1",
                PostalCity = "Kansas City",
                PostalState = "MO",
                PostalCountry = "USA",
                PostalZip = "64158",
                OrganizationId = newGuid
            });
            context.Organizations.Add(new Organization()
            {
                OrganizationId = newGuid,
                Name = "Org1",
                Ssn = "111-11-1111"
            });


            newGuid = Guid.NewGuid();

            context.Phones.Add(new Phone()
            {
                PhoneNumber = "(222) 222-2222",
                OrganizationId = newGuid
            });
            context.Addresses.Add(new Address()
            {
                Address1 = "Address1 Organization2",
                City = "Kansas City",
                State = "MO",
                Country = "USA",
                Zip = "64158",
                PostalAddress1 = "Address1 Organization2",
                PostalCity = "Kansas City",
                PostalState = "MO",
                PostalCountry = "USA",
                PostalZip = "64158",
                OrganizationId = newGuid
            });

            context.Organizations.Add(new Organization()
            {
                OrganizationId = newGuid,
                Name = "Org2",
                Ssn = "222-22-2222"
            });


            newGuid = Guid.NewGuid();

            context.Phones.Add(new Phone()
            {
                PhoneNumber = "(333) 333-3333",
                OrganizationId = newGuid
            });
            context.Addresses.Add(new Address()
            {
                Address1 = "Address1 Organization3",
                City = "Kansas City",
                State = "MO",
                Country = "USA",
                Zip = "64158",
                PostalAddress1 = "Address1 Organization3",
                PostalCity = "Kansas City",
                PostalState = "MO",
                PostalCountry = "USA",
                PostalZip = "64158",
                OrganizationId = newGuid
            });

            context.Organizations.Add(new Organization()
            {
                OrganizationId = newGuid,
                Name = "Org3",
                Ssn = "333-33-3333"
            });


            newGuid = Guid.NewGuid();
            context.Phones.Add(new Phone()
            {
                PhoneNumber = "(444) 444-4444",
                OrganizationId = newGuid
            });
            context.Addresses.Add(new Address()
            {
                Address1 = "Address1 Organization4",
                City = "Kansas City",
                State = "MO",
                Country = "USA",
                Zip = "64158",
                PostalAddress1 = "Address1 Organization4",
                PostalCity = "Kansas City",
                PostalState = "MO",
                PostalCountry = "USA",
                PostalZip = "64158",
                OrganizationId = newGuid
            });

            context.Organizations.Add(new Organization()
            {
                OrganizationId = newGuid,
                Name = "Org4",
                Ssn = "444-44-4444"
            });


            newGuid = Guid.NewGuid();

            context.Phones.Add(new Phone()
            {
                PhoneNumber = "(555) 555-5555",
                OrganizationId = newGuid
            });
            context.Addresses.Add(new Address()
            {
                Address1 = "Address1 Organization5",
                City = "Kansas City",
                State = "MO",
                Country = "USA",
                Zip = "64158",
                PostalAddress1 = "Address1 Organization5",
                PostalCity = "Kansas City",
                PostalState = "MO",
                PostalCountry = "USA",
                PostalZip = "64158",
                OrganizationId = newGuid
            });

            context.Organizations.Add(new Organization()
            {
                OrganizationId = newGuid,
                Name = "Org5",
                Ssn = "55-555-5555"
            });

            context.SaveChanges();

        }
        public static MapperConfiguration SetMapper()
        {
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

            return mappingConfig;

        }
    }

}
