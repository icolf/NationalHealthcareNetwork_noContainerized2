using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Organizations.Api.SmsServices.SmsModels;
using Twilio.Rest.Api.V2010.Account;

namespace Organizations.Api.SmsServices.SmsServices
{
    public interface ISmsService
    {
            Task<MessageResource> Send(SmsMessage smsMessage);
    }
}

