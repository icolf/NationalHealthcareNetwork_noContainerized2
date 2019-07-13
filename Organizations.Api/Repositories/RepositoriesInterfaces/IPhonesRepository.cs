using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Organizations.Api.Models;
using Organizations.Api.Models.CreationDtos;
using Organizations.Api.Persistence.Entities;

namespace Organizations.Api.Repositories.RepositoriesInterfaces
{
    public interface IPhonesRepository
    {
        bool IsPhoneExists(Guid organizationId, Guid phoneId);

        IEnumerable<PhoneDto> GetPhonesForOrganization(Guid organizationId);

        Phone GetPhone(Guid organizationId, Guid phoneId);

        Phone CreatePhone(Guid organizationId, [FromBody] PhoneForCreationDto phone);

        void DeletePhone(Guid organizationId, Guid phoneId);

        bool Save();
    }
}