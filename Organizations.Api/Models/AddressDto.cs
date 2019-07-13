using System;

namespace Organizations.Api.Models
{
    public class AddressDto
    {
        public Guid AddressId { get; set; }

        public Guid OrganizationId { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string Zip { get; set; }

        public string PostalAddress1 { get; set; }

        public string PostalAddress2 { get; set; }

        public string PostalState { get; set; }

        public string PostalCity { get; set; }

        public string PostalCountry { get; set; }

        public string PostalZip { get; set; }

    }
}