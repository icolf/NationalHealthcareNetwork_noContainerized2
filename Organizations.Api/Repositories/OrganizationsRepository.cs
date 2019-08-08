using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Organizations.Api.Helpers;
using Organizations.Api.Models;
using Organizations.Api.Models.CreationDtos;
using Organizations.Api.Models.UpdateDtos;
using Organizations.Api.Persistence;
using Organizations.Api.Persistence.Entities;
using Organizations.Api.Repositories.RepositoriesInterfaces;
using Organizations.Api.Services;

namespace Organizations.Api.Repositories
{
    public class OrganizationsRepository : IOrganizationsRepository
    {
        private readonly OrganizationsContext _context;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;

        public OrganizationsRepository(OrganizationsContext context, IMapper mapper, IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _mapper = mapper;
            _propertyMappingService = propertyMappingService;
        }
        public async Task<PageList<Organization>> GetOrganizations(OrganizationResourceParameters organizationResourceParameters)
        {
            var organizationBeforePaging =
                _context.Organizations.ApplySort(organizationResourceParameters.OrderBy, _propertyMappingService.GetPropertyMapping<OrganizationDto, Organization>());

            if (!string.IsNullOrEmpty(organizationResourceParameters.Name))
            {
                var descriptionForWhereClause = organizationResourceParameters.Name.Trim().ToLowerInvariant();
                organizationBeforePaging =
                    organizationBeforePaging.Where(o => o.Name.ToLowerInvariant().Contains(descriptionForWhereClause));
            }

            var organizations = await PageList<Organization>
                .Create(organizationBeforePaging
                    , organizationResourceParameters.CurrentPage
                    , organizationResourceParameters.PageSize
                    );
            return organizations;
        }

        public IEnumerable<Organization> GetOrganizationsOnly()
        {
            var organizations = _context.Organizations.ToList();
            return organizations;
        }

        public Organization GetOrganization(Guid? organizationId, bool includeChildren = false)
        {
            if (includeChildren)
            {
                return _context.Organizations
                    .Include(o => o.Addresses)
                    .Include(o => o.Phones)
                    .FirstOrDefault(o => o.OrganizationId == organizationId);
            }

            return _context.Organizations.FirstOrDefault(o => o.OrganizationId == organizationId);

        }

        public Organization CreateOrganization(OrganizationForCreationDto organization)
        {
            var organizationToAdd = _mapper.Map<Organization>(organization);

            _context.Organizations.Add(organizationToAdd);

            return organizationToAdd;
        }

        public void UpdateOrganization(OrganizationWithoutChildrenForUpdateDto organization, Guid organizationId)
        {

            var organizationToUpdate = _context.Organizations.FirstOrDefault(o => o.OrganizationId == organizationId);

           _mapper.Map(organization, organizationToUpdate);


            _context.SaveChanges();

        }

        public void DeleteOrganization(Organization organizationToDelete)
        {
            _context.Organizations.Remove(organizationToDelete);
        }

        public bool IsOrganizationExists(Guid organizationId)
        {
            return _context.Organizations.Any(o => o.OrganizationId == organizationId);
        }

        public void TrackOrganizationUpdate(Organization organization, OrganizationForUpdateDto organizationForUpdate)
        {
            var organizationWithoutChildren = _mapper.Map<OrganizationWithoutChildrenDto>(organizationForUpdate);
            _mapper.Map(organizationWithoutChildren, organization);
            _context.Organizations.Update(organization);
        }

        public Organization Find(Guid organizationId)
        {
            return _context.Organizations.Find(organizationId);
        }

    }
}
