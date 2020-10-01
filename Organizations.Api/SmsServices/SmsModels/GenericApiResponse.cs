using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organizations.Api.SmsServices.SmsModels
{
    public class GenericApiResponse
    {
        public GenericApiResponse(object response)
        {
            this.Response = response;
        }

        public object Response { get; private set; }
    }
}
