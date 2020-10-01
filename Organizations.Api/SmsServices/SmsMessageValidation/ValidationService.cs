using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;
using Organizations.Api.SmsServices.SmsModels;

namespace Organizations.Api.SmsServices.SmsMessageValidation
{
    public static class ValidationService
    {
        private static SendSmsMessageValidator validator = new SendSmsMessageValidator();

        public static List<string> Validate(out bool isValid, SmsMessage smsMessage)
        {
            if (smsMessage != null)
            {
                var validationResult = validator.Validate(smsMessage);

                isValid = validationResult.IsValid;

                return AggregateErrors(validationResult);
            }
            else
            {
                isValid = false;

                return new List<string> { "The submitted sms message is null." };
            }
        }

        private static List<string> AggregateErrors(ValidationResult validationResult)
        {
            var errors = new List<string>();

            if (!validationResult.IsValid)
            {
                Parallel.ForEach(validationResult.Errors, error =>
                {
                    errors.Add(error.ErrorMessage);
                });
            }

            return errors;
        }
    }
}
