using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organizations.Api.Models
{
    /// <summary>
    /// Base class for producing HATEOAS Links
    /// </summary>
    public abstract class LinkedResourceBaseDto
    {
        /// <summary>
        /// Collection of HATEOAS Links
        /// </summary>
        public List<LinkDto> Links { get; set; } = new List<LinkDto>();
    }
}
