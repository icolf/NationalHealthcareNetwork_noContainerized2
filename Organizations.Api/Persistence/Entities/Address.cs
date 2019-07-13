using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Organizations.Api.Persistence.Entities
{
    public class Address
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AddressId { get; set; }

        public Guid OrganizationId { get; set; }

        public Organization Organization { get; set; }

        [Required]
        [MaxLength(250)]
        public string Address1 { get; set; }

        [MaxLength(250)]
        public string Address2 { get; set; }

        [Required]
        [MaxLength(250)]
        public string State { get; set; }

        [Required]
        [MaxLength(250)]
        public string City { get; set; }

        [Required]
        [MaxLength(250)]
        public string Country { get; set; }

        [Required]
        [MaxLength(250)]
        public string Zip { get; set; }

        [Required]
        [MaxLength(250)]
        public string PostalAddress1 { get; set; }

        [MaxLength(250)]
        public string PostalAddress2 { get; set; }

        [Required]
        [MaxLength(250)]
        public string PostalState { get; set; }

        [Required]
        [MaxLength(250)]
        public string PostalCity { get; set; }

        [Required]
        [MaxLength(250)]
        public string PostalCountry { get; set; }

        [Required]
        [MaxLength(250)]
        public string PostalZip { get; set; }

    }
}
