using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Services.Exceptions
{
    public class InvalidReservationException : Exception
    {
        public InvalidReservationException()
        {
        }

        public InvalidReservationException(string message) : base(message)
        {
        }
    }
}
