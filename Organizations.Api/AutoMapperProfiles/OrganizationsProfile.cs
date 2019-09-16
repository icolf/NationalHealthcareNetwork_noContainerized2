using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Organizations.Api.Models;
using Organizations.Api.Models.CreationDtos;
using Organizations.Api.Models.UpdateDtos;
using Organizations.Api.Persistence.Entities;

namespace Organizations.Api.AutoMapperProfiles
{
    public class OrganizationsProfile : Profile
    {

        public OrganizationsProfile()
        {
            CreateMap<Organization, OrganizationWithoutChildrenDto>();
            CreateMap<Organization, OrganizationDto>();
            CreateMap<Organization, OrganizationForUpdateDto>();
            CreateMap<OrganizationDto, Organization>();
            CreateMap<OrganizationForCreationDto, Organization>();
            CreateMap<OrganizationForUpdateDto, Organization>();
            CreateMap<OrganizationWithoutChildrenDto, Organization>();
            CreateMap<OrganizationForUpdateDto, OrganizationWithoutChildrenDto>();
            CreateMap<OrganizationWithoutChildrenForUpdateDto, Organization>();
            CreateMap<OrganizationDto, OrganizationWithoutChildrenDto>().ReverseMap();
            
            CreateMap<Address, AddressDto>();
            CreateMap<Phone, PhoneDto>();
            CreateMap<PhoneForCreationDto, Phone>();
            CreateMap<AddressForCreationDto, Address>();
            CreateMap<AddressForUpdateDto, Address>();
            CreateMap<PhoneForUpdateDto, Phone>();
            CreateMap<Address, AddressForUpdateDto>();
            CreateMap<Phone, PhoneForUpdateDto>();

        }

        //private void AddOrUpdateAddresses(OrganizationForUpdateDto organizationForUpdateDto, Organization organization)
        //{
        //   foreach (var address in organizationForUpdateDto.Addresses)
        //    {
        //        if (address.AddressId == new Guid())
        //        {
        //            organization.Addresses.Add(Mapper.Map<Address>(address));
        //        }
        //        else
        //        {
        //            Mapper.Map(address, organization.Addresses.FirstOrDefault(a => a.AddressId == address.AddressId));
        //        }
        //    }
        //}
    }
}
