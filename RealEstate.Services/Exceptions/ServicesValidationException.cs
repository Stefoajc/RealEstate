using System;

namespace RealEstate.Services.Exceptions
{
    public class ServicesValidationException : Exception
    {
        public ServicesValidationException()
        {
        }

        public ServicesValidationException(string message) : base(message)
        {
        }
    }
}