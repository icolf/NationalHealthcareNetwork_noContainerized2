using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Organizations.Api.Persistence.Entities;

namespace Organizations.Api.Persistence.Entities
{
    public class Organization
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid OrganizationId { get; set; }

        public bool IsHospital { get; set; }

        [Required]
        public string Ssn { get; set; }

        [MaxLength(250)]
        [Required]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Email { get; set; }

        public List<Address> Addresses { get; set; } = new List<Address>();

        public List<Phone> Phones { get; set; } = new List<Phone>();

        [MaxLength(250)]
        public string Description { get; set; }

        [MaxLength(250)]
        public string Url { get; set; }

        [MaxLength(250)]
        public string ImagePath { get; set; }

        public int ActiveAddressId { get; set; }

    }
}
