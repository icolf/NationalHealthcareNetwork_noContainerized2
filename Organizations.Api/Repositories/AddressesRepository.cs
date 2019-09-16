using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organizations.Api.Models;
using Organizations.Api.Models.CreationDtos;
using Organizations.Api.Models.UpdateDtos;
using Organizations.Api.Persistence;
using Organizations.Api.Persistence.Entities;
using Organizations.Api.Repositories.RepositoriesInterfaces;

namespace Organizations.Api.Repositories
{
    public class AddressesRepository : IAddressesRepository
    {
        private readonly OrganizationsContext _context;
        private readonly IMapper _mapper;

        public AddressesRepository(OrganizationsContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public bool IsOrganizationExists(Guid organizationId)
        {
            return _context.Organizations.Any(o => o.OrganizationId == organizationId);
        }

        public bool IsAddressExists(Guid organizationId, Guid addressId)
        {
            return _context.Addresses.Any(a => a.OrganizationId == organizationId && a.AddressId == addressId);
        }

        public IEnumerable<AddressDto> GetAddressesForOrganization(Guid organizationId)
        {
            var organization = _context.Organizations.Include(o => o.Addresses)
                .FirstOrDefault(o => o.OrganizationId == organizationId);

            return _mapper.Map<IEnumerable<AddressDto>>(organization.Addresses);
        }

        public Address GetAddress(Guid organizationId, Guid addressId)
        {
            var addressFromContext = _context.Addresses
                .FirstOrDefault(a => a.OrganizationId == organizationId && a.AddressId == addressId);
            return addressFromContext;
        }


        public Address CreateAddress(Guid organizationId, AddressForCreationDto address)
        {
            var organizationFromContext =
                _context.Organizations.FirstOrDefault(o => o.OrganizationId == organizationId);
            var mappedAddress = _mapper.Map<Address>(address);

            organizationFromContext.Addresses.Add(mappedAddress);

            return mappedAddress;
        }

        public void DeleteAddress(Guid organizationId, Guid addressId)
        {
            var addressToDelete = GetAddress(organizationId, addressId);

            _context.Addresses.Remove(addressToDelete);
        }

        public void DeleteAddresses(OrganizationForUpdateDto organization, Organization organizationFromContext)
        {
            if (organization.DeletedAddresses.Count > 0)
            {
                foreach (var deletedAddress in organization.DeletedAddresses)
                {
                    foreach (var address in organizationFromContext.Addresses)
                    {
                        if (address.AddressId == deletedAddress.AddressId)
                        {
                            _context.Addresses.Remove(address);
                        }
                    }
                }
            }
        }
        public void UpdateAndAddAddresses(List<AddressForUpdateDto> addresses, List<Address> addressesFromContext, Guid organizationId)
        {
            foreach (var updatedAddress in addresses)
            {
                if (updatedAddress.AddressId == new Guid())
                {
                    updatedAddress.OrganizationId = organizationId;
                    _context.Addresses.Add(_mapper.Map<AddressForUpdateDto, Address>(updatedAddress));
                }
                else
                {
                    foreach (var address in addressesFromContext)
                    {
                        if (address.AddressId == updatedAddress.AddressId)
                        {
                            updatedAddress.OrganizationId = organizationId;
                            _mapper.Map<AddressForUpdateDto, Address>(updatedAddress, address);
                            _context.Addresses.Update(address);
                        }
                    }
                }
            }

        }


        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
