using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organizations.Api.Models.UpdateDtos
{
    public class OrganizationWithoutChildrenForUpdateDto
    {
        public bool IsHospital { get; set; }

        public string Ssn { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public string ImagePath { get; set; }

        public int ActiveAddressId { get; set; }
    }
}
