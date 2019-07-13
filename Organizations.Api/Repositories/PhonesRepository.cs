using Organizations.Api.Repositories.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Organizations.Api.Models;
using Organizations.Api.Models.CreationDtos;
using Organizations.Api.Persistence;
using Organizations.Api.Persistence.Entities;

namespace Organizations.Api.Repositories
{
    public class PhonesRepository: IPhonesRepository
    {
        private readonly OrganizationsContext _context;
        private readonly IMapper _mapper;

        public PhonesRepository(OrganizationsContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public bool IsPhoneExists(Guid organizationId, Guid phoneId)
        {
            return _context.Addresses.Any(a => a.OrganizationId == organizationId && a.AddressId == phoneId);
        }

        public IEnumerable<PhoneDto> GetPhonesForOrganization(Guid organizationId)
        {
            var organization = _context.Organizations.Include(o => o.Phones)
                .FirstOrDefault(o => o.OrganizationId == organizationId);

            return _mapper.Map<IEnumerable<PhoneDto>>(organization.Addresses);
        }

        public Phone GetPhone(Guid organizationId, Guid phoneId)
        {
            var phoneFromContext = _context.Phones
                .FirstOrDefault(a => a.OrganizationId == organizationId && a.PhoneId == phoneId);
            return phoneFromContext;

        }

        public Phone CreatePhone(Guid organizationId, PhoneForCreationDto phone)
        {
            var organizationFromContext =
                _context.Organizations.FirstOrDefault(o => o.OrganizationId == organizationId);
            var mappedPhone = _mapper.Map<Phone>(phone);

            organizationFromContext.Phones.Add(_mapper.Map<Phone>(mappedPhone));

            return mappedPhone;
        }

        public void DeletePhone(Guid organizationId, Guid phoneId)
        {
            var phoneToDelete = GetPhone(organizationId, phoneId);

            _context.Phones.Remove(phoneToDelete);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
