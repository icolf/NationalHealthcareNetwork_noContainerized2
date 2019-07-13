using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Organizations.Api.Models.CreationDtos
{
    public class OrganizationForCreationDto
    {

        public bool IsHospital { get; set; }

        [Required(ErrorMessage = "Social Security number is required")]
        public string Ssn { get; set; }

        [Required(ErrorMessage = "Address missing it is required")]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Bad Email format")]
        public string Email { get; set; }

        public List<AddressForCreationDto> Addresses { get; set; } = new List<AddressForCreationDto>();

        public List<PhoneForCreationDto> Phones { get; set; } = new List<PhoneForCreationDto>();

        public string Description { get; set; }

        public string Url { get; set; }

        public string ImagePath { get; set; }

        public int ActiveAddressId { get; set; }
    }
}
