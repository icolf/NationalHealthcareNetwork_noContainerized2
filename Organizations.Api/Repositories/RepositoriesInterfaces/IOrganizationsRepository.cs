using System;
using System.Collections.Generic;
using System.Linq;
using Organizations.Api.Helpers;
using Organizations.Api.Models;
using Organizations.Api.Models.CreationDtos;
using Organizations.Api.Models.UpdateDtos;
using Organizations.Api.Persistence.Entities;

namespace Organizations.Api.Repositories.RepositoriesInterfaces
{
    public interface IOrganizationsRepository
    {
        PageList<Organization> GetOrganizations(OrganizationResourceParameters organizationResourceParameters);

        IEnumerable<Organization> GetOrganizationsOnly();

        Organization GetOrganization(Guid? organizationId, bool includeChildren);

        Organization CreateOrganization(OrganizationForCreationDto organization);

        void UpdateOrganization(OrganizationWithoutChildrenForUpdateDto organization, Guid organizationId);

        void DeleteOrganization(Organization organizationToDelete);

        bool IsOrganizationExists(Guid organizationId);

        void TrackOrganizationUpdate(Organization organization, OrganizationForUpdateDto organizationForUpdate);

        Organization Find(Guid organizationId);

    }
}