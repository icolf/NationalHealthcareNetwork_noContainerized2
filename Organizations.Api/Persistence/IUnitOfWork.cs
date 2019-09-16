using Organizations.Api.Repositories.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organizations.Api.Persistence
{
    public interface IUnitOfWork
    {
        IOrganizationsRepository Organizations { get; set; }
        IAddressesRepository Addresses { get; set; }
        IPhonesRepository   Phones { get; set; }

        bool Complete();
    }
}
