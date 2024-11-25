using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Exceptions
{
    public class ProductNotFoundException : Exception
    {
        public ProductNotFoundException(string id)
            : base($"History of product ID: {id} was not found")
        {
        }

        public ProductNotFoundException()
            : base($"History of products was not found")
        {
        }
    }
}
