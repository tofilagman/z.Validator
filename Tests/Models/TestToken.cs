using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using z.Validator.Attributes;

namespace z.Validator.Test.Models
{
    public class TestToken
    {

        [IsRequired]
        public string LastName { get; set; }

    }
}
