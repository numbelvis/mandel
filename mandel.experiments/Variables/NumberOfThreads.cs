using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mandel.experiments
{
    public class NumberOfThreads : ValueTypeVariable<int>
    {
        public string Title
        {
            get { return "Number of Threads"; }
        }


    }
}
