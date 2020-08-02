using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Services.Exceptions
{
    public class ContentNotFoundException : Exception
    {
        public ContentNotFoundException()
        {
            
        }

        public ContentNotFoundException(string message):base(message)
        {
            
        }
    }

    public class UserNotFoundException : Exception
    {
        public UserNotFoundException()
        {
            
        }

        public UserNotFoundException(string message):base(message)
        {
            
        }
    }

    public class NotAuthorizedException : Exception
    {
        public NotAuthorizedException()
        {
            
        }

        public NotAuthorizedException(string message) : base(message)
        {
            
        }
    }

    public class UniquenessException : Exception
    {
        public UniquenessException()
        {
        }

        public UniquenessException(string message) : base(message)
        {
        }
    }
}
