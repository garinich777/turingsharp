using System;

namespace TuringSharp.Parser
{
    public class ParserException : Exception
    {

        private const string _Message = "Error while while parsing input";

        public ParserException(string message)
            : base(message)
        {
            this.LineNumber = null;
        }

        public ParserException(string message, int lineNumber) : base(message)
        {
            this.LineNumber = lineNumber;
        }

        public int? LineNumber { get; set; }

    }
}
