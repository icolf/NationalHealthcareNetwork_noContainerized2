using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
    public class OrganizationsRepository : IOrganizationsRepository
    {
        private readonly OrganizationsContext _context;
        private readonly IMapper _mapper;

        public OrganizationsRepository(OrganizationsContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public IEnumerable<OrganizationWithoutChildrenDto> GetOrganizations()
        {
            var organizations = _context.Organizations.OrderBy(o=>o.Name)
                .Include(o => o.Addresses)
                .Include(o => o.Phones)
                .ToList();

            var organizationsToReturn = _mapper.Map<IEnumerable<OrganizationWithoutChildrenDto>>(organizations);

            return organizationsToReturn;
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

            _context.SaveChanges();

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
            _context.SaveChanges();
        }

        public bool IsOrganizationExists(Guid organizationId)
        {
            return _context.Organizations.Any(o => o.OrganizationId == organizationId);
        }


    }
}
