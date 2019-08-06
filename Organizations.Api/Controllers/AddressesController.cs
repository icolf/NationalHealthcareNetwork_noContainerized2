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
        private readonly IUrlHelper _urlHelper;

        public AddressesController(IUnitOfWork unitOfWork, IMapper mapper, IUrlHelper urlHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _urlHelper = urlHelper;
        }

        [HttpGet("{organizationId}/addresses", Name = "GetAddressesForOrganization")]
        public IActionResult GetAddresses(Guid organizationId)
        {
            if (!_unitOfWork.Addresses.IsOrganizationExists(organizationId))
            {
                return NotFound();
            }

            var addressesToReturn = _unitOfWork.Addresses.GetAddressesForOrganization(organizationId);

            addressesToReturn = addressesToReturn.Select(address =>
            {
                address = CreateLinksForAddress(address);
                return address;
            });

            var wrapper=new LinkedCollectionResourceWrapperDto<AddressDto>(addressesToReturn);

            return Ok(CreateLinksForAddresses(wrapper));
        }

        [HttpGet("{organizationId}/addresses/{addressId}", Name ="GetAddressForOrganization")]
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

            var addressFromContext = _unitOfWork.Addresses.GetAddress(organizationId, addressId);

            var addressToReturn = _mapper.Map<AddressDto>(addressFromContext);

            if (!_unitOfWork.Complete())
            {
                return StatusCode(500, "A problem happened while handling your request!");
            }

            return Ok(CreateLinksForAddress(addressToReturn));
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

            var addressToReturn = _mapper.Map<AddressDto>(mappedAddress);


            return CreatedAtRoute("GetAddressForOrganization",
                new {organizationId = organizationId, addressId = addressToReturn.AddressId}, CreateLinksForAddress(addressToReturn));
        }

        [HttpPut("{organizationId}/addresses/{addressId}", Name = "UpdateAddressForOrganization")]
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

        [HttpDelete("{organizationId}/addresses/{addressId}", Name = "DeleteAddressForOrganization")]
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

        [HttpPatch("{organizationId}/addresses/{addressId}", Name = "PartiallyUpdateAddressForOrganization")]
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

        private AddressDto CreateLinksForAddress(AddressDto address)
        {
            address.Links.Add(new LinkDto(_urlHelper.Link("GetAddressForOrganization", new {addressId = address.AddressId}),
                "self"
                ,"GET"));
            address.Links.Add(new LinkDto(_urlHelper.Link("UpdateAddressForOrganization", new { addressId = address.AddressId}),
                "update_address"
                ,"PUT"));
            address.Links.Add(new LinkDto(_urlHelper.Link("DeleteAddressForOrganization", new { addressId = address.AddressId}),
                "delete_address"
                , "DELETE"));
            address.Links.Add(new LinkDto(_urlHelper.Link("PartiallyUpdateAddressForOrganization", new { addressId = address.AddressId}),
                "partially_update_address"
                , "PATCH"));
            return address;
        }

        private LinkedCollectionResourceWrapperDto<AddressDto> CreateLinksForAddresses(
            LinkedCollectionResourceWrapperDto<AddressDto> addressesWrapper)
        {
            addressesWrapper.Links.Add(
                new LinkDto(_urlHelper.Link("GetAddressesForOrganization", new { })
                , "self",
                "GET"));
            return addressesWrapper;
        }
    }
}
