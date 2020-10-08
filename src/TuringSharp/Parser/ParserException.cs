using System;

namespace TuringSharp.Parser
{
    public class ParserException : Exception
    {

        private const string _Message = "Error while parsing input";

        public ParserException(string message)
            : base(message)
        {
            LineNumber = null;
        }

        public ParserException(string message, int lineNumber) : base(message)
        {
            LineNumber = lineNumber;
        }

        public int? LineNumber { get; set; }
    }
}
