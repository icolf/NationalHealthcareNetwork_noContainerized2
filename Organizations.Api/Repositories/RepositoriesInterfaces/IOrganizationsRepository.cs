using System;
using System.Collections.Generic;
using System.Linq;
using Organizations.Api.Models;
using Organizations.Api.Models.CreationDtos;
using Organizations.Api.Models.UpdateDtos;
using Organizations.Api.Persistence.Entities;

namespace Organizations.Api.Repositories.RepositoriesInterfaces
{
    public interface IOrganizationsRepository
    {
        IEnumerable<OrganizationWithoutChildrenDto> GetOrganizations();

        IEnumerable<Organization> GetOrganizationsOnly();

        Organization GetOrganization(Guid? organizationId, bool includeChildren);

        Organization CreateOrganization(OrganizationForCreationDto organization);

        void UpdateOrganization(OrganizationWithoutChildrenForUpdateDto organization, Guid organizationId);

        void DeleteOrganization(Organization organizationToDelete);

        bool IsOrganizationExists(Guid organizationId);

    }
}