using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organizations.Api.Persistence.Entities
{
    public class ParentChildRelation
    {
        public Guid ParentId { get; set; }

        public Guid ChildId { get; set; }
    }
}
