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
        private readonly OrganizationsContext _context;
        private IOrganizationsRepository _organizationsRepository;
        private readonly IMapper _mapper;

        public OrganizationsController (OrganizationsContext context, IOrganizationsRepository organizationsRepository, IMapper mapper)
        {
            _context = context;
            _organizationsRepository = organizationsRepository;
            _mapper = mapper;
        }

        [HttpGet()]
        public IActionResult GetOrganizations()
        {
            var organizations = _organizationsRepository.GetOrganizations();

            return Ok(organizations);
        }

        [HttpGet("{orgOnly}")]
        public IActionResult GetOrganizationsOnly(bool orgOnly)
        {
            var organizations = _organizationsRepository.GetOrganizationsOnly();
            return Ok(organizations);
        }

        [HttpGet("{organizationId}", Name ="GetOrganization")]
        public IActionResult GetOrganization(Guid? organizationId, bool includeChildren=false)
        {

            var organization =_organizationsRepository.GetOrganization(organizationId, includeChildren);

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


            var organizationToAdd = _organizationsRepository.CreateOrganization(organizationDto);

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

            if (!_organizationsRepository.IsOrganizationExists(organizationId))
            {
                return NotFound();
            }

            _organizationsRepository.UpdateOrganization(organization, organizationId);

            return NoContent();

        }

        [HttpDelete("{organizationId}")]
        public IActionResult DeleteOrganization(Guid organizationId)
        {
            if (organizationId == null)
            {
                return BadRequest();
            }

            var organizationToDelete = _context.Organizations.FirstOrDefault(o => o.OrganizationId == organizationId);

            if (organizationToDelete == null)
            {
                return NotFound();
            }

            _organizationsRepository.DeleteOrganization(organizationToDelete);

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

            var organizationToUpdate = _context.Organizations.FirstOrDefault(o => o.OrganizationId == organizationId);

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

            _context.Attach(organizationToUpdate);

            _context.SaveChanges();
            return NoContent();



        }
    }
}
