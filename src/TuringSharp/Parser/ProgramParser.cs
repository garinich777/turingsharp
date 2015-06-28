using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSharp.CodeDom;
using TuringSharp.Runtime;

namespace TuringSharp.Parser
{
    /// <summary>
    /// Loads a program from a string
    /// </summary>
    public class ProgramParser
    {

        public Program Parse(string input)
        {
            var program = new Program();

            using (var reader = new StringReader(input))
            {
                string line = null;
                int lineNumber = 0;

                // Read until the end of file
                while ((line = reader.ReadLine()) != null)
                {
                    // Trim whitespaces. DO NOT call this method in the while loop as the method ReadLine() returns null at the EOF
                    line = line.Trim();

                    // Keep track of the line number
                    lineNumber++;

                    // Ignore blank lines and comments
                    if (line.Length == 0 || line.StartsWith("//") || line.StartsWith(";"))
                        continue;
                    else
                    {
                        // Ignore double whitespaces
                        var fields = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (fields.Length < 5)
                            throw new ParserException("Not enough fields", lineNumber);

                        // Do not check for blank fields because they have already been excluded by the Split function
                        Statement statement = null;

                        // Try to build a statement
                        try
                        {
                            statement = new Statement()
                            {
                                CurrentState = ValidateAndReturnStateName(fields[0]),
                                CurrentSymbol = ValidateAndReturnSymbol(fields[1]),
                                NewSymbol = ValidateAndReturnSymbol(fields[2]),
                                Direction = ValidateAndReturnDirection(fields[3]),
                                NewState = ValidateAndReturnStateName(fields[4]),
                                LineNumber = lineNumber
                            };
                        }
                        catch(ParserException ex)
                        {
                            // Supply additional informations
                            ex.LineNumber = lineNumber;
                            throw;
                        }

                        // Check for duplicate rules
                        if (program.Statements.Where(s => s.CurrentState == statement.CurrentState && s.CurrentSymbol == statement.CurrentSymbol).Any())
                            throw new ParserException("Duplicate rule", lineNumber);

                        // Add statement to program
                        program.Statements.Add(statement);
                    }
                }
            }

            return program;
        }

        private string ValidateAndReturnStateName(string name)
        {
            // No validation needed
            return name;
        }

        private char ValidateAndReturnSymbol(string symbol)
        {
            if (symbol.Length != 1)
                throw new ParserException("This is not a valid symbol");

            return symbol[0];
        }

        private TapeDirection ValidateAndReturnDirection(string direction)
        {
            switch (direction)
            {
                case "r":
                    return TapeDirection.Right;
                case "l":
                    return TapeDirection.Left;
                case "*":
                case "s":
                    return TapeDirection.Still;
                default:
                    throw new ParserException("Invalid direction");
            }
        }

    }

}
