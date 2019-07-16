using System;
using System.Collections.Generic;
using System.Linq;
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
    public class OrganizationsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrganizationsController (IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet()]
        public IActionResult GetOrganizations()
        {
            var organizations = _unitOfWork.Organizations.GetOrganizations();

            return Ok(organizations);
        }

        [HttpGet("{orgOnly}")]
        public IActionResult GetOrganizationsOnly(bool orgOnly)
        {
            var organizations = _unitOfWork.Organizations.GetOrganizationsOnly();
            return Ok(organizations);
        }

        [HttpGet("{organizationId}", Name ="GetOrganization")]
        public IActionResult GetOrganization(Guid? organizationId, bool includeChildren=false)
        {

            var organization =_unitOfWork.Organizations.GetOrganization(organizationId, includeChildren);

            if (organization == null)
            {
                return NotFound();
            }

            if (includeChildren)
            {
                var organizationToReturn = _mapper.Map<OrganizationDto>(organization);

                return Ok(organizationToReturn);
            }

            return Ok(_mapper.Map<OrganizationWithoutChildrenDto>(organization));
        }

        [HttpPost()]
        public IActionResult CreateOrganization([FromBody] OrganizationForCreationDto organizationDto)
        {
            if (organizationDto == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var organizationToAdd = _unitOfWork.Organizations.CreateOrganization(organizationDto);

            return CreatedAtRoute("GetOrganization", new {organizationId = organizationToAdd.OrganizationId},
                organizationToAdd);

        }

        [HttpPut("{organizationId}")]
        public IActionResult UpdateOrganization(Guid organizationId, [FromBody] OrganizationWithoutChildrenForUpdateDto organization)
        {
            if (organization == null)
            {
                return BadRequest();
            }

            if (organizationId == new Guid())
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_unitOfWork.Organizations.IsOrganizationExists(organizationId))
            {
                return NotFound();
            }

            _unitOfWork.Organizations.UpdateOrganization(organization, organizationId);

            return NoContent();

        }

        [HttpDelete("{organizationId}")]
        public IActionResult DeleteOrganization(Guid organizationId)
        {
            if (organizationId == null)
            {
                return BadRequest();
            }

            var organizationToDelete = _unitOfWork.Organizations.GetOrganization(organizationId, false);

            if (organizationToDelete == null)
            {
                return NotFound();
            }

            _unitOfWork.Organizations.DeleteOrganization(organizationToDelete);

            return NoContent();
        }

        [HttpPatch("{organizationId}")]
        public IActionResult UpdateOrganization(Guid organizationId,
            [FromBody] JsonPatchDocument<OrganizationForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var organizationToUpdate = _unitOfWork.Organizations.GetOrganization(organizationId, false);

            if (organizationToUpdate == null)
            {
                return NotFound();
            }

            var organizationToPatch = _mapper.Map<OrganizationForUpdateDto>(organizationToUpdate);

            patchDoc.ApplyTo(organizationToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TryValidateModel(organizationToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(organizationToPatch, organizationToUpdate);

            if (!_unitOfWork.Complete())
            {
                return StatusCode(500, "A problem happened while handling your request!");
            }
            return NoContent();



        }
    }
}
