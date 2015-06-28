using System.Collections.Generic;

namespace TuringSharp.CodeDom
{

    /// <summary>
    /// Describes a program executable by a turing machine
    /// </summary>
    public class Program
    {

        public Program()
        {
            Statements = new List<Statement>();
        }

        public List<Statement> Statements { get; set; }

    }

}
