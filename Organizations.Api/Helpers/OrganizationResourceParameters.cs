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

        /// <summary>
        /// Rows in a page
        /// </summary>
        public int PageSize  
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }

        /// <summary>
        /// Actual Page
        /// </summary>
        public int CurrentPage { get; set; } = 1;

        /// <summary>
        /// Default field to Order By
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Default field to apply Order By
        /// </summary>
        public string OrderBy { get; set; } = "Name";

        /// <summary>
        /// CSV of fields included in Order By command
        /// </summary>
        public string Fields { get; set; }

    }
}
