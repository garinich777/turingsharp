using System.Diagnostics;
using TuringSharp.Runtime;

namespace TuringSharp.CodeDom
{
    [DebuggerDisplay("{CurrentState} {CurrentSymbol} {NewSymbol} {Direction} {NewState}")]
    public class Statement
    {

        public const char AnySymbol = '*';

        /// <summary>
        /// Used for debugging. Indicates the line number in the original source code.
        /// </summary>
        public int? LineNumber { get; set; }

        public string CurrentState { get; set; }

        public char CurrentSymbol { get; set; }

        public char NewSymbol { get; set; }

        public TapeDirection Direction { get; set; }

        public string NewState { get; set; }

    }
}
