using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mandel.arb
{
    public class ArbAdditionOverflow : OverflowException
    {
        public override string Message
        {
            get
            {
                return "An ARB ADDITION operation caused whole number information to be lost.  The number was too large.";
            }
        }
    }
}
