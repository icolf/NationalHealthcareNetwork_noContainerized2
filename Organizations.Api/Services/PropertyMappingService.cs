using System;
using System.Collections.Generic;
using System.Linq;
using NLog.Time;
using Organizations.Api.Models;
using Organizations.Api.Persistence.Entities;

namespace Organizations.Api.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _organizationPropertyMapping = 
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Id", new PropertyMappingValue(new List<string>(){"Id"})},
                {"Name", new PropertyMappingValue(new List<string>(){"Name"})},
                {"Description", new PropertyMappingValue(new List<string>(){"Description"}) }
            };

        private IList<IPropertyMapping> propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            propertyMappings.Add(new PropertyMapping<OrganizationDto, Organization>(_organizationPropertyMapping));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMapping = propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact mapping instance for <{typeof(TSource)}, {typeof(TDestination)}>");
        }
    }
}
