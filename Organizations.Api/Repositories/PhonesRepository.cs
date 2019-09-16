using Organizations.Api.Repositories.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Organizations.Api.Models;
using Organizations.Api.Models.CreationDtos;
using Organizations.Api.Models.UpdateDtos;
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
            return _context.Phones.Any(a => a.OrganizationId == organizationId && a.PhoneId == phoneId);
        }

        public IEnumerable<PhoneDto> GetPhonesForOrganization(Guid organizationId)
        {
            var organization = _context.Organizations.Include(o => o.Phones)
                .FirstOrDefault(o => o.OrganizationId == organizationId);

            return _mapper.Map<IEnumerable<PhoneDto>>(organization.Phones);
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

        public void DeletePhones(OrganizationForUpdateDto organization, Organization organizationFromContext)
        {
            if (organization.DeletedPhones.Count > 0)
            {
                foreach (var deletedPhone in organization.DeletedPhones)
                {
                    foreach (var phone in organizationFromContext.Phones)
                    {
                        if (phone.PhoneId == deletedPhone.PhoneId)
                        {
                            _context.Phones.Remove(phone);
                        }
                    }
                }
            }

        }

        public void UpdateAndAddPhones(List<PhoneForUpdateDto> phones, List<Phone> phonesFromContext, Guid organizationId)
        {
            foreach (var updatedPhone in phones)
            {
                if (updatedPhone.PhoneId == new Guid())
                {
                    updatedPhone.OrganizationId = organizationId;
                    _context.Phones.Add(_mapper.Map<PhoneForUpdateDto, Phone>(updatedPhone));
                }
                else
                {
                    foreach (var phone in phonesFromContext)
                    {
                        if (phone.PhoneId == updatedPhone.PhoneId)
                        {
                            updatedPhone.OrganizationId = organizationId;
                            _mapper.Map<PhoneForUpdateDto, Phone>(updatedPhone, phone);
                            _context.Phones.Update(phone);
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
