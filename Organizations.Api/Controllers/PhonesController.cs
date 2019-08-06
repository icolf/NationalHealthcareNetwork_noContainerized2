using System;
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
using Organizations.Api.Repositories.RepositoriesInterfaces;

namespace Organizations.Api.Controllers
{
    [Route("Api/organizations")]
    public class PhonesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public PhonesController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        [HttpGet("{organizationId}/phones")]
        public IActionResult GetPhones(Guid organizationId)
        {
            if (!_unitOfWork.Organizations.IsOrganizationExists(organizationId))
            {
                return NotFound();
            }


            var phones = _unitOfWork.Phones.GetPhonesForOrganization(organizationId);

            if (phones == null)
            {
                return NotFound();
            }

            return Ok(phones);
        }

        [HttpGet("{organizationId}/phones/{phoneId}", Name = "GetPhone")]
        public IActionResult GetPhone(Guid organizationId, Guid phoneId)
        {
            if (organizationId == new Guid())
            {
                return BadRequest();
            }

            if (!_unitOfWork.Organizations.IsOrganizationExists(organizationId))
            {
                return NotFound();
            }

            if (!_unitOfWork.Phones.IsPhoneExists(organizationId, phoneId))
            {
                return NotFound();
            }

            var phoneToReturn = _unitOfWork.Phones.GetPhone(organizationId, phoneId);

            _mapper.Map<PhoneDto>(phoneToReturn);

            return Ok(phoneToReturn);
        }

        [HttpPost("{organizationId}/phones")]
        public IActionResult CreatePhone(Guid organizationId, [FromBody] PhoneForCreationDto phone)
        {
            if (organizationId == new Guid())
            {
                return BadRequest();
            }

            if (phone == null)
            {
                return BadRequest();
            }

            if (!_unitOfWork.Organizations.IsOrganizationExists(organizationId))
            {
                return NotFound();
            }

            var mappedPhone = _unitOfWork.Phones.CreatePhone(organizationId, phone);

            if (!_unitOfWork.Complete())
            {
                return StatusCode(500, "A problem happened while handling your request!");
            }

            var phoneToReturn = _mapper.Map<PhoneDto>(mappedPhone);

            return CreatedAtRoute("GetPhone",
                new { organizationId = organizationId, phoneId = phoneToReturn.PhoneId }, phoneToReturn);
        }

        [HttpDelete("{organizationId}/phones/{phoneId}")]
        public IActionResult DeletePhone(Guid organizationId, Guid phoneId)
        {
            if (organizationId == new Guid())
            {
                return BadRequest();
            }

            if (phoneId == new Guid())
            {
                return BadRequest();
            }

            if (!_unitOfWork.Organizations.IsOrganizationExists(organizationId))
            {
                return NotFound();
            }

            if (!_unitOfWork.Phones.IsPhoneExists(organizationId, phoneId))
            {
                return NotFound();
            }

            _unitOfWork.Phones.DeletePhone(organizationId, phoneId);

            if (!_unitOfWork.Complete())
            {
                return StatusCode(500, "A problem happened while handling your request!");
            }

            return NoContent();
        }

        [HttpPut("{organizationId}/phones/{phoneId}")]
        public IActionResult UpdatePhone(Guid organizationId, Guid phoneId, [FromBody] PhoneForUpdateDto phoneForUpdate)
        {
            if (organizationId == new Guid())
            {
                return BadRequest();
            }

            if (phoneId == new Guid())
            {
                return BadRequest();
            }

            if (!_unitOfWork.Organizations.IsOrganizationExists(organizationId))
            {
                return NotFound();
            }

            if (!_unitOfWork.Phones.IsPhoneExists(organizationId, phoneId))
            {
                return NotFound();
            }

            var phoneFromContext = _unitOfWork.Phones.GetPhone(organizationId, phoneId);

            _mapper.Map(phoneForUpdate, phoneFromContext);

            if (!_unitOfWork.Complete())
            {
                return StatusCode(500, "A problem happened while handling your request!");
            }

            return NoContent();
        }

        [HttpPatch("{organizationId}/phones/{phoneId}")]
        public IActionResult UpdatePhone(Guid organizationId, Guid phoneId,
            [FromBody] JsonPatchDocument<PhoneForUpdateDto> jsonPatchDocument)
        {
            if (organizationId == new Guid())
            {
                return BadRequest();
            }

            if (phoneId == new Guid())
            {
                return BadRequest();
            }

            if (!_unitOfWork.Organizations.IsOrganizationExists(organizationId))
            {
                return NotFound();
            }

            if (!_unitOfWork.Phones.IsPhoneExists(organizationId, phoneId))
            {
                return NotFound();
            }

            var phoneFromContext = _unitOfWork.Phones.GetPhone(organizationId, phoneId);

            var phoneToPatch = _mapper.Map<PhoneForUpdateDto>(phoneFromContext);
            jsonPatchDocument.ApplyTo(phoneToPatch, ModelState);

            TryValidateModel(phoneToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(phoneToPatch, phoneFromContext);

            if (!_unitOfWork.Complete())
            {
                return StatusCode(500, "A problem happened while handling your request!");
            }

            return NoContent();
        }




    }
}
