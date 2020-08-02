using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Services.Exceptions
{
    public class NotAuthorizedUserException : Exception
    {
        public NotAuthorizedUserException():base()
        {
            
        }

        public NotAuthorizedUserException(string message) : base(message)
        {
            
        }
    }
}
