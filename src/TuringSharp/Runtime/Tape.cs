using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSharp.Runtime
{
    public class Tape
    {

        public const char Blank = '_';
        public const int GrowthFactor = 5; // Grow by 50 items
        public const int InitialSize = 4;

        private int pointer = 0;

        public Tape()
        {
            Data = new char[InitialSize];
            FillWithBlanks(0, InitialSize);
        }

        private char[] Data { get; set; }

        public int Pointer
        {
            get
            {
                return pointer;
            }
        }

        public char CurrentSymbol
        {
            get
            {
                return Data[pointer];
            }
            set
            {
                Data[pointer] = value;
            }
        }

        public bool MoveRight()
        {
            bool didGrow = false;

            if (pointer >= (Data.Length - 1))
            {
                // Need to the extend the tape on the right
                Grow(true);
                didGrow = true;
            }

            pointer++;
            return didGrow;
        }

        public bool MoveLeft()
        {
            bool didGrow = false;

            if (pointer == 0)
            {
                Grow(false);
                didGrow = true;
            }

            pointer--;
            return didGrow;
        }

        private void Grow(bool onRightSideOfHead, int by = GrowthFactor)
        {
            var tempData = Data;
            Data = new char[Data.Length + by];

            if (onRightSideOfHead)
            {
                // Grow on the right
                tempData.CopyTo(Data, 0);

                // Fill with blanks
                FillWithBlanks(Data.Length - by, by);
            }
            else
            {
                // Extend tape on the left
                tempData.CopyTo(Data, by);

                // Fill with blanks
                FillWithBlanks(0, by);

                // Need to re-index the pointer
                pointer = GrowthFactor;
            }
        }

        /// <summary>
        /// Fills the tape with blank characters in the specified range
        /// </summary>
        private void FillWithBlanks(int index, int count)
        {
            for (int i = index; i < index + count; i++)
            {
                Data[i] = Blank;
            }
        }

        /// <summary>
        /// Initializes the tape with initial data
        /// </summary>
        public void InitializeWithData(string input)
        {
            Data = input.ToCharArray();
        }

        /// <summary>
        /// Reads the tape 'fromLeft' symbols on the left side of the currently pointer symbols (excluded)
        /// and 'fromRight' symbols on the right side of the currently pointer symbol (included!)
        /// </summary>
        /// <param name="fromLeft">Number of symbols to read on the left side of the pointer (excluded)</param>
        /// <param name="fromRight">Number of symbols to read on the right side of the pointer (included)</param>
        /// <example>
        /// If the tape content is HELLO_WORLD and the currently pointed symbol is '_' then:
        /// GetCellsAsString(0,1) returns "_"
        /// GetCellsAsString(1,1) returns "O_"
        /// GetCellsAsString(3,2) returns "LLO_WO"
        /// </example>
        public string GetCellsAsString(int fromLeft, int fromRight)
        {
            StringBuilder result = new StringBuilder(fromLeft + fromRight);

            if (fromLeft > pointer)
            {
                // If there are not enough symbols on the left, fill with blanks
                result.Append(new string(Tape.Blank, (fromLeft - pointer)));
                // Now append the symbols on the left of the head
                result.Append(Data.Take(pointer).ToArray());
            }
            else
                // Read the required number of symbols from the left
                result.Append(Data.Skip(pointer - fromLeft).Take(fromLeft).ToArray());


            if (fromRight > (Data.Length - pointer))
            {
                // Not enough symbols on the right side, so first read all the available symbols
                result.Append(Data.Skip(pointer).Take(Data.Length - pointer).ToArray());
                // ... and then fill with blanks
                result.Append(new string(Tape.Blank, (fromRight - (Data.Length - pointer))));
            }
            else
                // Just read the symbols on the right
                result.Append(Data.Skip(pointer).Take(fromRight).ToArray());


            return result.ToString();
        }

    }
}
