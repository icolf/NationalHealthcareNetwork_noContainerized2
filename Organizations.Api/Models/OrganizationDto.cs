using System;
using System.Collections.Generic;

namespace Organizations.Api.Models
{
    /// <summary>
    /// An organization with multiple properties, phones, and addresses
    /// </summary>
    public class OrganizationDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// Id for the organization
        /// </summary>
        public Guid OrganizationId { get; set; }
        /// <summary>
        /// Is the organization a hospital?
        /// </summary>
        public bool IsHospital { get; set; }

        /// <summary>
        /// Tax identification number
        /// </summary>
        public string Ssn { get; set; }

        /// <summary>
        /// Name for the organization
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Official email for the organization
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Collection of valid addresses for the organization 
        /// </summary>
        public List<AddressDto> Addresses { get; set; } = new List<AddressDto>();

        /// <summary>
        /// Collection of valid phones for the organization 
        /// </summary>
        public List<PhoneDto> Phones { get; set; } = new List<PhoneDto>();

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Page URL for the organization
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        ///Uri of a picture of the organization 
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// Address that is active
        /// </summary>
        public int ActiveAddressId { get; set; }

    }

}
