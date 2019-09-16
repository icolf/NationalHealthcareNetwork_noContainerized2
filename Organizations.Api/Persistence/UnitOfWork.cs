using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Organizations.Api.Repositories;
using Organizations.Api.Repositories.RepositoriesInterfaces;
using Organizations.Api.Services;

namespace Organizations.Api.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OrganizationsContext _context;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        public IOrganizationsRepository Organizations { get; set; }
        public IAddressesRepository Addresses { get; set; }
        public IPhonesRepository Phones { get; set; }

        public UnitOfWork(OrganizationsContext context, IMapper mapper, IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _mapper = mapper;
            _propertyMappingService = propertyMappingService;
            Organizations = new OrganizationsRepository(_context, _mapper, _propertyMappingService);
            Addresses = new AddressesRepository(_context, _mapper);
            Phones = new PhonesRepository(_context, _mapper);
        }
        public bool Complete()
        {
            return (_context.SaveChanges()>=0);
        }
    }
}
