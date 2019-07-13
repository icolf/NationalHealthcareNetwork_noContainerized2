using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Organizations.Api.Enums;

namespace Organizations.Api.Persistence.Entities
{
    public class Phone
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PhoneId { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public Guid OrganizationId { get; set; }

        public Organization Organization { get; set; }

        public string Extension { get; set; }

        public string Type { get; set; }

        public bool IsForDisplay { get; set; }
    }
}