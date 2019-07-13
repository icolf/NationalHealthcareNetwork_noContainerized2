using System;
using Organizations.Api.Enums;

namespace Organizations.Api.Models
{
    public class PhoneDto
    {
        public Guid PhoneId { get; set; }

        public string PhoneNumber { get; set; }

        public string Extension { get; set; }

        public string Type { get; set; }

        public bool IsForDisplay { get; set; }

        public Guid OrganizationId { get; set; }
    }
}