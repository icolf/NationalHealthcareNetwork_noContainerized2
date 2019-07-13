using System;
using System.Collections;
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
using Organizations.Api.Repositories;
using Organizations.Api.Repositories.RepositoriesInterfaces;

namespace Organizations.Api.Controllers
{
    [Route("api/organizations")]
    public class AddressesController : ControllerBase
    {
        private readonly OrganizationsContext _context;
        private readonly IAddressesRepository _addressesRepository;
        private readonly IMapper _mapper;

        public AddressesController(OrganizationsContext context, IAddressesRepository addressesRepository, IMapper mapper)
        {
            _context = context;
            _addressesRepository = addressesRepository;
            _mapper = mapper;
        }

        [HttpGet("{organizationId}/addresses")]
        public IActionResult GetAddresses(Guid organizationId)
        {
            if (!_addressesRepository.IsOrganizationExists(organizationId))
            {
                return NotFound();
            }

            var addressesToReturn = _addressesRepository.GetAddressesForOrganization(organizationId);

            return Ok(addressesToReturn);
        }

        [HttpGet("{organizationId}/addresses/{addressId}", Name ="GetAddress")]
        public IActionResult GetAddress(Guid organizationId, Guid addressId)
        {
            if (organizationId == new Guid())
            {
                return BadRequest();
            }

            if (!_addressesRepository.IsOrganizationExists(organizationId))
            {
                return NotFound();
            }

            if (!_addressesRepository.IsAddressExists(organizationId, addressId))
            {
                return NotFound();
            }

            var addressToReturn = _addressesRepository.GetAddress(organizationId, addressId);

            _mapper.Map<AddressDto>(addressToReturn);

            return Ok(addressToReturn);
        }

        [HttpPost("{organizationId}/addresses")]
        public IActionResult CreateAddress(Guid organizationId, [FromBody] AddressForCreationDto address)
        {
            if (organizationId == new Guid())
            {
                return BadRequest();
            }

            if (address == null)
            {
                return BadRequest();
            }

            if (!_addressesRepository.IsOrganizationExists(organizationId))
            {
                return NotFound();
            }

            var mappedAddress= _addressesRepository.CreateAddress(organizationId, address);

            if (!_addressesRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request!");
            }

            return CreatedAtRoute("GetAddress",
                new {organizationId = organizationId, addressId = mappedAddress.AddressId}, mappedAddress);
        }

        [HttpPut("{organizationId}/addresses/{addressId}")]
        public IActionResult UpdateAddress(Guid organizationId, Guid addressId, [FromBody] AddressForUpdateDto addressForUpdate)
        {
            if (organizationId == new Guid())
            {
                return BadRequest();
            }

            if (addressId == new Guid())
            {
                return BadRequest();
            }

            if (!_addressesRepository.IsOrganizationExists(organizationId))
            {
                return NotFound();
            }

            var addressFromContext = _context.Addresses.FirstOrDefault(a => a.AddressId == addressId);
            if (addressFromContext == null)
            {
                return NotFound();
            }

            _mapper.Map(addressForUpdate,addressFromContext);

            if (!_addressesRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request!");
            }

            return NoContent();
        }

        [HttpDelete("{organizationId}/addresses/{addressId}")]
        public IActionResult DeleteAddress(Guid organizationId, Guid addressId)
        {
            if (organizationId == new Guid())
            {
                return BadRequest();
            }

            if (addressId == new Guid())
            {
                return BadRequest();
            }

            if (!_addressesRepository.IsOrganizationExists(organizationId))
            {
                return NotFound();
            }

            if (!_addressesRepository.IsAddressExists(organizationId, addressId))
            {
                return NotFound();
            }

            _addressesRepository.DeleteAddress(organizationId, addressId);

            if (!_addressesRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request!");
            }

            return Ok();
        }

        [HttpPatch("{organizationId}/addresses/{addressId}")]
        public IActionResult UpdateAddress(Guid organizationId, Guid addressId,
            [FromBody] JsonPatchDocument<AddressForUpdateDto> jsonPatchDocument)
        {
            if (organizationId == new Guid())
            {
                return BadRequest();
            }

            if (addressId == new Guid())
            {
                return BadRequest();
            }

            if (!_addressesRepository.IsOrganizationExists(organizationId))
            {
                return NotFound();
            }

            if (!_addressesRepository.IsAddressExists(organizationId, addressId))
            {
                return NotFound();
            }

            var addressFromContext = _addressesRepository.GetAddress(organizationId, addressId);

            var addressToPatch = _mapper.Map<AddressForUpdateDto>(addressFromContext);
            jsonPatchDocument.ApplyTo(addressToPatch, ModelState);

            TryValidateModel(addressToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(addressToPatch, addressFromContext);

            if (!_addressesRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request!");
            }

            return NoContent();
        }
    }
}
