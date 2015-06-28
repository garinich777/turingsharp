using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSharp.Runtime
{
    public class IncompleteProgramException : Exception
    {

        public IncompleteProgramException() : base("There is no rule for this state and symbol.")
        {

        }

        public string State { get; set; }

        public char CurrentSymbol { get; set; }

    }
}
