using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organizations.Api.Helpers
{
    public class OrganizationResourceParameters
    {
        private int _pageSize = 10;
        const int maxPageSize = 20;

        public int PageSize  
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }

        public int CurrentPage { get; set; } = 1;

        public string Name { get; set; }

        public string OrderBy { get; set; } = "Name";

        public string Fields { get; set; }

    }
}
