using Organizations.Api.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Organizations.Api.Models.UpdateDtos
{
    public class OrganizationForUpdateDto
    {
        public Guid OrganizationId { get; set; }
        public bool IsHospital { get; set; } = false;

        [Required(ErrorMessage = "A valid SS is Required")]
        public string Ssn { get; set; }

        [Required(ErrorMessage = "A Name is Required")]
        public string Name { get; set; }

        public string Email { get; set; }

        public List<AddressForUpdateDto> Addresses { get; set; } = new List<AddressForUpdateDto>();

        public List<AddressDto> DeletedAddresses { get; set; } = new List<AddressDto>();

        public List<PhoneForUpdateDto> Phones { get; set; } = new List<PhoneForUpdateDto>();

        public List<PhoneDto> DeletedPhones { get; set; } = new List<PhoneDto>();

        [Required(ErrorMessage = "A Description is Required")]
        public string Description { get; set; }

        public string Url { get; set; }

        public TrackedStatus Status { get; set; }

        public string ImagePath { get; set; }

        public int ActiveAddressId { get; set; }

    }
}


