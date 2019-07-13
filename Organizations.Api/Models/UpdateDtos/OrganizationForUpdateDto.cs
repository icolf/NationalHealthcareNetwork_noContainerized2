using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Organizations.Api.Models.UpdateDtos
{
    public class OrganizationForUpdateDto
    {
        public bool IsHospital { get; set; } = false;

        [Required(ErrorMessage = "A valid SS is Required")]
        public string Ssn { get; set; }

        [Required(ErrorMessage = "A Name is Required")]
        public string Name { get; set; }

        public string Email { get; set; }

        public List<AddressForUpdateDto> Addresses { get; set; } = new List<AddressForUpdateDto>();

        public List<PhoneForUpdateDto> Phones { get; set; } = new List<PhoneForUpdateDto>();

        [Required(ErrorMessage = "A Description is Required")]
        public string Description { get; set; }

        public string Url { get; set; }

        public string ImagePath { get; set; }

        public int ActiveAddressId { get; set; }

    }
}


