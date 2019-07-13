using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Organizations.Api.Persistence.Entities;
using Organizations.Api.Models;

namespace Organizations.Api.Models
{
    public class OrganizationDto
    {
        public Guid OrganizationId { get; set; }

        public bool IsHospital { get; set; }

        public string Ssn { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public List<AddressDto> Addresses { get; set; } = new List<AddressDto>();

        public List<PhoneDto> Phones { get; set; } = new List<PhoneDto>();

        public string Description { get; set; }

        public string Url { get; set; }

        public string ImagePath { get; set; }

        public int ActiveAddressId { get; set; }

    }

}
