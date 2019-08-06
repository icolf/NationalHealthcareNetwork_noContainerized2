using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organizations.Api.Services
{
    public interface IPropertyMappingService
    {
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
    }
}
