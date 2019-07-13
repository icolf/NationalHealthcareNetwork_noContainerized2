using System;
using System.ComponentModel.DataAnnotations;

namespace Organizations.Api.Models.UpdateDtos
{
    public class PhoneForUpdateDto
    {
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        public string Extension { get; set; }

        public string Type { get; set; }

        public bool IsForDisplay { get; set; }

        [Required(ErrorMessage = "Organization Id is required")]
        public Guid OrganizationId { get; set; }
    }
}