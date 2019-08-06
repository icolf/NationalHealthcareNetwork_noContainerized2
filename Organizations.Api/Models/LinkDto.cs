using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organizations.Api.Models
{
    public class LinkDto
    {
        public string Href { get; set; }

        public string Rel { get; set; }

        public string Method { get; set; }

        public LinkDto(string href, string rel, string method)
        {
            Method = method;
            Rel = rel;
            Href = href;
        }
    }
}
