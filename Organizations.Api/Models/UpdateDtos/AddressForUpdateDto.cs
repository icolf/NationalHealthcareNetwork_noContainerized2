using Organizations.Api.Persistence.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Organizations.Api.Enums;

namespace Organizations.Api.Models.UpdateDtos
{
    public class AddressForUpdateDto
    {
        public Guid AddressId { get; set; }

        [Required(ErrorMessage = "Organization Id is required")]
        public Guid OrganizationId { get; set; }

        [Required(ErrorMessage = "First line for address is required")]
        public string Address1 { get; set; }

        public string Address2 { get; set; }

        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Zip Code is required")]
        public string Zip { get; set; }

        public string PostalAddress1 { get; set; }

        public string PostalAddress2 { get; set; }

        public string PostalState { get; set; }

        public string PostalCity { get; set; }

        public string PostalCountry { get; set; }

        public string PostalZip { get; set; }

        public TrackedStatus Status { get; set; }

    }
}