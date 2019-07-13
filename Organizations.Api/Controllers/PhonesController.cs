using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organizations.Api.Models;
using Organizations.Api.Models.CreationDtos;
using Organizations.Api.Persistence;
using Organizations.Api.Repositories.RepositoriesInterfaces;

namespace Organizations.Api.Controllers
{
    [Route("Api/organizations")]
    public class PhonesController : ControllerBase
    {
        private readonly OrganizationsContext _ctx;
        private readonly IMapper _mapper;
        private readonly IPhonesRepository _phonesRepository;
        private readonly IOrganizationsRepository _organizationRepository;

        public PhonesController(OrganizationsContext ctx, IMapper mapper, IPhonesRepository phonesRepository, IOrganizationsRepository organizationRepository)
        {
            _ctx = ctx;
            _mapper = mapper;
            _phonesRepository = phonesRepository;
            _organizationRepository = organizationRepository;
        }
        [HttpGet("{organizationId}/phones")]
        public IActionResult GetPhones(Guid organizationId)
        {
            if (!_organizationRepository.IsOrganizationExists(organizationId))
            {
                return NotFound();
            }


            var phones = _phonesRepository.GetPhonesForOrganization(organizationId);

            if (phones == null)
            {
                return NotFound();
            }

            return Ok(phones);
        }

        [HttpGet("{organizationId}/phones/{phoneId}", Name = "GetPhone")]
        public IActionResult GetAddress(Guid organizationId, Guid phoneId)
        {
            if (organizationId == new Guid())
            {
                return BadRequest();
            }

            if (!_organizationRepository.IsOrganizationExists(organizationId))
            {
                return NotFound();
            }

            if (!_phonesRepository.IsPhoneExists(organizationId, phoneId))
            {
                return NotFound();
            }

            var phoneToReturn = _phonesRepository.GetPhone(organizationId, phoneId);

            _mapper.Map<PhoneDto>(phoneToReturn);

            return Ok(phoneToReturn);
        }

        [HttpPost("{organizationId}/phones")]
        public IActionResult CreatePhone(Guid organizationId, [FromBody] PhoneForCreationDto phone)
        {
            if (organizationId == new Guid());
            {
                return BadRequest();
            }

            if (phone == null)
            {
                return BadRequest();
            }

            if (!_organizationRepository.IsOrganizationExists(organizationId))
            {
                return NotFound();
            }

            var mappedAddress = _phonesRepository.CreatePhone(organizationId, phone);

            if (!_phonesRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request!");
            }

            return CreatedAtRoute("GetPhone",
                new { organizationId = organizationId, addressId = mappedAddress.PhoneId }, mappedAddress);
        }


    }
}
