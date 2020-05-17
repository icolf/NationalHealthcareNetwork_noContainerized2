using Microsoft.EntityFrameworkCore;
using Organizations.Api.Persistence.Entities;
using System;

namespace Organizations.Api.Persistence
{
    public class OrganizationsContext  :DbContext
    {
        public OrganizationsContext(DbContextOptions options) : base(options)
        {
            Database.Migrate();
        }

        public DbSet<Organizations.Api.Persistence.Entities.Organization> Organizations { get; set; }
        public DbSet<Phone> Phones { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Organization>().HasData(
                new Organization()
                {
                    OrganizationId = new Guid("123e4567-e89b-12d3-a456-426655440001"),
                    Name = "Organization1",
                    Description = "Description for Organization 1",
                    Ssn="000000001"
                },
                new Organization()
                {
                    OrganizationId = new Guid("123e4567-e89b-12d3-a456-426655440002"),
                    Name = "Organization1",
                    Description = "Description for Organization 2",
                    Ssn = "000000002"
                }
            );
            modelBuilder.Entity<Phone>().HasData(
                new Phone()
                {
                    PhoneId = new Guid("123e4567-e89b-12d3-a456-426655440003"),
                    PhoneNumber = "1234567890",
                    OrganizationId = Guid.Parse("123e4567-e89b-12d3-a456-426655440001"),
                    Extension = "123"
                },
                new Phone()
                {
                    PhoneId = new Guid("123e4567-e89b-12d3-a456-426655440004"),
                    PhoneNumber = "1234567890",
                    OrganizationId = Guid.Parse("123e4567-e89b-12d3-a456-426655440001"),
                    Extension = "123"
                },
                new Phone()
                {
                    PhoneId = new Guid("123e4567-e89b-12d3-a456-426655440005"),
                    PhoneNumber = "1234567890",
                    OrganizationId = Guid.Parse("123e4567-e89b-12d3-a456-426655440002"),
                    Extension = "123"
                }
            );
            modelBuilder.Entity<Address>().HasData(
                new Address()
                {
                    AddressId=new Guid("123e4567-e89b-12d3-a456-426655440006"),
                    Address1="Address1 Organization1",
                    City="Kansas City",
                    State="MO",
                    Country="USA",
                    Zip="64158",
                    PostalAddress1="Address1 Organization1",
                    PostalCity="Kansas City",
                    PostalState="MO",
                    PostalCountry="USA",
                    PostalZip="64158",
                    OrganizationId=Guid.Parse("123e4567-e89b-12d3-a456-426655440001")
                },
                new Address()
                {
                    AddressId = new Guid("123e4567-e89b-12d3-a456-426655440007"),
                    Address1 = "Address1 Organization2",
                    City = "Kansas City",
                    State = "MO",
                    Country = "USA",
                    Zip = "64158",
                    PostalAddress1 = "Address1 Organization2",
                    PostalCity = "Kansas City",
                    PostalState = "MO",
                    PostalCountry = "USA",
                    PostalZip = "64158",
                    OrganizationId = Guid.Parse("123e4567-e89b-12d3-a456-426655440002")
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
