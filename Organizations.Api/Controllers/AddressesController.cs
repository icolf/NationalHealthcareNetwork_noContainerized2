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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AddressesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("{organizationId}/addresses")]
        public IActionResult GetAddresses(Guid organizationId)
        {
            if (!_unitOfWork.Addresses.IsOrganizationExists(organizationId))
            {
                return NotFound();
            }

            var addressesToReturn = _unitOfWork.Addresses.GetAddressesForOrganization(organizationId);

            return Ok(addressesToReturn);
        }

        [HttpGet("{organizationId}/addresses/{addressId}", Name ="GetAddress")]
        public IActionResult GetAddress(Guid organizationId, Guid addressId)
        {
            if (organizationId == new Guid())
            {
                return BadRequest();
            }

            if (!_unitOfWork.Addresses.IsOrganizationExists(organizationId))
            {
                return NotFound();
            }

            if (!_unitOfWork.Addresses.IsAddressExists(organizationId, addressId))
            {
                return NotFound();
            }

            var addressToReturn = _unitOfWork.Addresses.GetAddress(organizationId, addressId);

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

            if (!_unitOfWork.Addresses.IsOrganizationExists(organizationId))
            {
                return NotFound();
            }

            var mappedAddress= _unitOfWork.Addresses.CreateAddress(organizationId, address);

            if (!_unitOfWork.Complete())
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

            if (!_unitOfWork.Addresses.IsOrganizationExists(organizationId))
            {
                return NotFound();
            }

            if (!_unitOfWork.Addresses.IsAddressExists(organizationId, addressId))
            {
                return NotFound();
            }

            var addressFromContext = _unitOfWork.Addresses.GetAddress(organizationId, addressId);

            _mapper.Map(addressForUpdate,addressFromContext);

            if (!_unitOfWork.Complete())
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

            if (!_unitOfWork.Addresses.IsOrganizationExists(organizationId))
            {
                return NotFound();
            }

            if (!_unitOfWork.Addresses.IsAddressExists(organizationId, addressId))
            {
                return NotFound();
            }

            _unitOfWork.Addresses.DeleteAddress(organizationId, addressId);

            if (!_unitOfWork.Complete())
            {
                return StatusCode(500, "A problem happened while handling your request!");
            }

            return NoContent();
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

                if (!_unitOfWork.Addresses.IsOrganizationExists(organizationId))
                {
                    return NotFound();
                }

                if (!_unitOfWork.Addresses.IsAddressExists(organizationId, addressId))
                {
                    return NotFound();
                }

                var addressFromContext = _unitOfWork.Addresses.GetAddress(organizationId, addressId);

                var addressToPatch = _mapper.Map<AddressForUpdateDto>(addressFromContext);
                jsonPatchDocument.ApplyTo(addressToPatch, ModelState);

                TryValidateModel(addressToPatch);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _mapper.Map(addressToPatch, addressFromContext);

                if (!_unitOfWork.Complete())
                {
                    return StatusCode(500, "A problem happened while handling your request!");
                }

                return NoContent();
            }
    }
}
