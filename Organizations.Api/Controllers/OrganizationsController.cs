using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
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


        /// <summary>
        /// Gets all Organizations with Pagination Meta-Data and HATEOAS Links
        /// </summary>
        /// <param name="organizationResourceParameters">Type that contains parameter for X-pagination header</param>
        /// <param name="mediaType">Use to set the  Http Accept Header to "Application/Json" or "application/json-NHN+json" that is used for HATEOAS links</param>
        /// <returns>List of organizations</returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(IEnumerable<OrganizationDto>))]
        [HttpGet(Name="GetOrganizations")]
        public async Task<IActionResult> GetOrganizations(OrganizationResourceParameters organizationResourceParameters, [FromHeader(Name = "Accept")] string mediaType)
        {
            //Adding a comment in dev branch to trigger a CI after making a PR to master branch
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

            var organizationsToReturn = _mapper.Map<IEnumerable<OrganizationDto>>(organizations);


            organizationsToReturn = organizationsToReturn.Select(o =>
            {
                o = CreateLinksForOrganization(o);
                return o;
            });

            var organizationsToReturnWithoutChildren = _mapper.Map<IEnumerable<OrganizationWithoutChildrenDto>>(organizationsToReturn);

            return Ok(organizationsToReturnWithoutChildren.ShapeData(organizationResourceParameters.Fields));
        }



        /// <summary>
        /// Get an organization by its Id
        /// </summary>
        /// <param name="organizationId">The Id of the organization you want to get</param>
        /// <param name="includeChildren">If true the result will include all children (phones and addresses</param>
        /// <returns>An ActionResult of type Organization</returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrganizationDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{organizationId}", Name ="GetOrganization")]
        public IActionResult GetOrganization(Guid? organizationId, bool includeChildren=false)
        {

            var organization =_unitOfWork.Organizations.GetOrganization(organizationId, includeChildren);

            if (organization == null)
            {
                return NotFound();
            }

            var organizationToReturn = _mapper.Map<OrganizationDto>(organization);
            organizationToReturn = CreateLinksForOrganization(organizationToReturn);

            if (includeChildren)
            {

                return Ok(organizationToReturn);
            }

            _logger.LogInformation(100, $"Organization Id= {organizationId} Name= {organization.Name} return.");

            var organizationWithoutChildren = _mapper.Map<OrganizationWithoutChildrenDto>(organizationToReturn);
            return Ok(_mapper.Map<OrganizationWithoutChildrenDto>(organizationWithoutChildren));
        }


        /// <summary>
        /// Creates a new Organization
        /// </summary>
        /// <param name="organization">OrganizationForCreationDto</param>
        /// <returns>OrganizationForCreationDto</returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost(Name = "CreateOrganization")]
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

            var organizationToReturn = CreateLinksForOrganization(_mapper.Map<OrganizationDto>(organizationToAdd));

            return CreatedAtRoute("GetOrganization", new {organizationId = organizationToReturn.OrganizationId},
                organizationToReturn);

        }

        /// <summary>
        /// Update an Organization
        /// </summary>
        /// <param name="organizationId">Id for the Organization</param>
        /// <param name="organization">OrganizationForUpdateDto</param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPut("{organizationId}", Name="UpdateOrganization")]
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


        /// <summary>
        /// Delete an Organization using its Id
        /// </summary>
        /// <param name="organizationId">The Id of the organization to be deleted</param>
        /// <returns></returns>

        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpDelete("{organizationId}", Name = "DeleteOrganization")]
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
        /// <summary>
        /// Partially update an Organization
        /// </summary>
        /// <param name="organizationId">Id of the Organization</param>
        /// <param name="patchDoc">The set of operations to apply to an Organization</param>
        /// <returns>Http result</returns>
        /// <remarks>
        /// Sample request (this request updates the organization name)\
        /// PATCH /organizations/organizationId\
        /// [\
        ///     {\
        ///         "op": "replace"\
        ///         "path": "/name"\
        ///         "value": "The best organization"\
        ///     }\
        /// ] \
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPatch("{organizationId}", Name = "PartiallyUpdateOrganization")]
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

        private OrganizationDto CreateLinksForOrganization(OrganizationDto organization)
        {
            organization.Links.Add(
                new LinkDto(_urlHelper.Link("GetOrganization", new {organization.OrganizationId}),
                "self",
               "Get"));
            organization.Links.Add(
                new LinkDto(_urlHelper.Link("UpdateOrganization", new {organization.OrganizationId}),
                "update_organization",
               "Put"));
            organization.Links.Add(
                new LinkDto(_urlHelper.Link("DeleteOrganization", new {organization.OrganizationId}),
                "delete_organization",
               "Delete"));
            organization.Links.Add(
                new LinkDto(_urlHelper.Link("PartiallyUpdateOrganization", new {organization.OrganizationId}),
                "update_organization",
               "Patch"));
            return organization;
        }

    }
}
