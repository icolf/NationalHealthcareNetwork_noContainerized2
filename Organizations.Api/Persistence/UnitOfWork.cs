using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Organizations.Api.Repositories;
using Organizations.Api.Repositories.RepositoriesInterfaces;

namespace Organizations.Api.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OrganizationsContext _context;
        private readonly IMapper _mapper;
        public IOrganizationsRepository Organizations { get; set; }
        public IAddressesRepository Addresses { get; set; }
        public IPhonesRepository Phones { get; set; }

        public UnitOfWork(OrganizationsContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            Organizations = new OrganizationsRepository(_context, _mapper);
            Addresses = new AddressesRepository(_context, _mapper);
            Phones = new PhonesRepository(_context, _mapper);
        }
        public bool Complete()
        {
            return (_context.SaveChanges()>=0);
        }
    }
}
