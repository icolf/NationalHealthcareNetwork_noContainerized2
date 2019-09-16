using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organizations.Api.Models
{
    /// <summary>
    /// Dto for HATEOAS
    /// </summary>
    public class LinkDto
    {
        /// <summary>
        /// Uri for a HATEOAS Link
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        /// Relation with object
        /// </summary>
        public string Rel { get; set; }

        /// <summary>
        /// Http command type
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="href"></param>
        /// <param name="rel"></param>
        /// <param name="method"></param>
        public LinkDto(string href, string rel, string method)
        {
            Method = method;
            Rel = rel;
            Href = href;
        }
    }
}
