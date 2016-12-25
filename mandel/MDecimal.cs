using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mandel
{
    public class MDecimal
    {
        public decimal value = 0m;

        public MDecimal(decimal value)
        {
            this.value = value;
        }

        public static implicit operator MDecimal(decimal dec)
        {
            return new MDecimal(dec);
        }
    }
}
