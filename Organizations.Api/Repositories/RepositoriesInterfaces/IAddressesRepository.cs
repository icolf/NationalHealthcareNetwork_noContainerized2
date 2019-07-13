using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Organizations.Api.Models;
using Organizations.Api.Models.CreationDtos;
using Organizations.Api.Models.UpdateDtos;
using Organizations.Api.Persistence.Entities;

namespace Organizations.Api.Repositories.RepositoriesInterfaces
{
    public interface IAddressesRepository
    {
        bool IsOrganizationExists(Guid organizationId);

        IEnumerable<AddressDto> GetAddressesForOrganization(Guid organizationId);

        Address GetAddress(Guid organizationId, Guid addressId);

        bool IsAddressExists(Guid organizationId, Guid addressId);

        Address CreateAddress(Guid organizationId, AddressForCreationDto address);

        void DeleteAddress(Guid organizationId, Guid addressId);

        bool Save();
    }
}
