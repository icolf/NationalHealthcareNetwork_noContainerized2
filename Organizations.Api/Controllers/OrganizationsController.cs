using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Organizations.Api.Enums;
using Organizations.Api.Helpers;
using Organizations.Api.Models;
using Organizations.Api.Models.CreationDtos;
using Organizations.Api.Models.UpdateDtos;
using Organizations.Api.Persistence;
using Organizations.Api.Services;
// ReSharper disable IdentifierTypo
using UnprocessableEntityObjectResult = Microsoft.AspNetCore.Mvc.UnprocessableEntityObjectResult;
// ReSharper restore IdentifierTypo

namespace Organizations.Api.Controllers
{
    [Route("api/organizations")]

    public class OrganizationsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OrganizationsController> _logger;
        private readonly IUrlHelper _urlHelper;
        private readonly ITypeHelperServices _typeHelperServices;

        public OrganizationsController (IUnitOfWork unitOfWork, IMapper mapper, ILogger<OrganizationsController> logger, IUrlHelper urlHelper, ITypeHelperServices typeHelperServices)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _urlHelper = urlHelper;
            _typeHelperServices = typeHelperServices;
        }

        [HttpGet(Name="GetOrganizations")]
        public async Task<IActionResult> GetOrganizations(OrganizationResourceParameters organizationResourceParameters)
        {

            var organizations = await _unitOfWork.Organizations.GetOrganizations(organizationResourceParameters);

            if (!_typeHelperServices.TypeHasProperties<OrganizationDto>(organizationResourceParameters.Fields))
            {
                return BadRequest();
            }



            //Pagination Metadata
            var previousPageLink = organizations.HasPrevious
                ? CreateOrganizationsResourceUri(organizationResourceParameters, ResourceUriType.PreviousPage)
                : null;
            var nextPageLink = organizations.HasNext
                ? CreateOrganizationsResourceUri(organizationResourceParameters, ResourceUriType.NextPage)
                : null;

            var paginationMetadata = new
            {
                totalCount = organizations.TotalCount,
                pageSize = organizations.PageSize,
                currentPage = organizations.CurrentPage,
                totalPages = organizations.TotalPages,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

            var organizationsToReturn = _mapper.Map<IEnumerable<OrganizationWithoutChildrenDto>>(organizations);



            return Ok(organizationsToReturn.ShapeData(organizationResourceParameters.Fields));
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

            _logger.LogInformation(100, $"Organization Id= {organizationId} Name= {organization.Name} return.");


            return Ok(_mapper.Map<OrganizationWithoutChildrenDto>(organization));
        }

        [HttpPost()]
        public IActionResult CreateOrganization([FromBody] OrganizationForCreationDto organization)
        {
            if (organization == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }


            var organizationToAdd = _unitOfWork.Organizations.CreateOrganization(organization);

            if (!_unitOfWork.Complete())
            {
                return StatusCode(500, "A problem happened while handling your request!");
            }

            var organizationToReturn = _mapper.Map<OrganizationDto>(organizationToAdd);

            return CreatedAtRoute("GetOrganization", new {organizationId = organizationToReturn.OrganizationId},
                organizationToReturn);

        }


        [HttpPut("{organizationId}")]
        public IActionResult UpdateOrganization(Guid organizationId, [FromBody] OrganizationForUpdateDto organization)
        {
            if (organizationId == new Guid())
            {
                return BadRequest();
            }

            if(!_unitOfWork.Organizations.IsOrganizationExists(organizationId))
            {
                return NotFound();
            }

            if (organization == null)
            {
                return BadRequest();
            }

            var organizationFromContext = _unitOfWork.Organizations.GetOrganization(organizationId, true);

            _unitOfWork.Organizations.TrackOrganizationUpdate(organizationFromContext, organization);

            _unitOfWork.Phones.UpdateAndAddPhones(organization.Phones, organizationFromContext.Phones, organizationId);

            _unitOfWork.Addresses.UpdateAndAddAddresses(organization.Addresses, organizationFromContext.Addresses, organizationId);

            _unitOfWork.Phones.DeletePhones(organization, organizationFromContext);

            _unitOfWork.Addresses.DeleteAddresses(organization, organizationFromContext);

            if (!_unitOfWork.Complete())
            {
                return StatusCode(500, "A problem happened while handling your request!");
            }

            return NoContent();
        }

        [HttpDelete("{organizationId}")]
        public IActionResult DeleteOrganization(Guid organizationId)
        {

            if (organizationId == new Guid())
            {
                return BadRequest();
            }

            if (!_unitOfWork.Organizations.IsOrganizationExists(organizationId))
            {
                return NotFound();
            }

            var organizationToDelete = _unitOfWork.Organizations.GetOrganization(organizationId, false);

            _unitOfWork.Organizations.DeleteOrganization(organizationToDelete);

            if (!_unitOfWork.Complete())
            {
                return StatusCode(500, "A problem happened while handling your request!");
            }

            _logger.LogInformation(100, $"Organization Id= {organizationId} Name= {organizationToDelete.Name} was deleted.");

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

        private string CreateOrganizationsResourceUri(OrganizationResourceParameters organizationResourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetOrganizations", new
                    {
                        fields=organizationResourceParameters.Fields,
                        orderBy=organizationResourceParameters.OrderBy,
                        name=organizationResourceParameters.Name,
                        currentPage = organizationResourceParameters.CurrentPage + 1,
                        pageSize = organizationResourceParameters.PageSize
                    });
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetOrganizations", new
                    {
                        fields = organizationResourceParameters.Fields,
                        orderBy = organizationResourceParameters.OrderBy,
                        name = organizationResourceParameters.Name,
                        currentPage = organizationResourceParameters.CurrentPage - 1,
                        pageSize = organizationResourceParameters.PageSize
                    });
                default:
                    return _urlHelper.Link("GetOrganizations", new
                    {
                        fields = organizationResourceParameters.Fields,
                        orderBy = organizationResourceParameters.OrderBy,
                        name = organizationResourceParameters.Name,
                        currentPage = organizationResourceParameters.CurrentPage,
                        pageSize = organizationResourceParameters.PageSize
                    });
            }
        }

    }
}
