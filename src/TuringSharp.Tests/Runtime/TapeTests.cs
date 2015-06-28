using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.IO;
using TuringSharp.Runtime;

namespace TuringSharp.Tests.Runtime
{
    [TestClass]
    
    public class TapeTests
    {
        [TestMethod]
        [Description("Test without making the tape grow if the symbols are read/written correctly. First test the right side of the tape, then the left")]
        public void TestWithoutGrowth()
        {
            InternalTestWithoutGrowth(true); // Move right
            InternalTestWithoutGrowth(false); // Move left
        }

        [TestMethod]
        public void TestInitialize()
        {
            var t = new Tape();
            var testString = "HELLO WORLD";
            t.InitializeWithData(testString);

            StringBuilder builder = new StringBuilder(testString.Length);
            while (t.CurrentSymbol != Tape.Blank)
            {
                builder.Append(t.CurrentSymbol);
                t.MoveRight();
            }

            Assert.AreEqual(testString, builder.ToString());
        }

        [TestMethod()]
        public void TestReadCells()
        {
            var t = new Tape();
            var testString = "HELLO WORLD"; // Do not change the testString wihout changing the test cases because
            t.InitializeWithData(testString);

            // Move to the space character
            while (t.CurrentSymbol != ' ') t.MoveRight();

            // Test empty case.
            Assert.AreEqual("", t.GetCellsAsString(0, 0));

            // Test without adding additional blank characters
            Assert.AreEqual("HELLO", t.GetCellsAsString(5, 0));
            Assert.AreEqual(" WORLD", t.GetCellsAsString(0, 6));
            Assert.AreEqual(testString, t.GetCellsAsString(5, 6));

            // Try asking less characters than those available.
            Assert.AreEqual("O", t.GetCellsAsString(1, 0));
            Assert.AreEqual("LO", t.GetCellsAsString(2, 0));
            Assert.AreEqual("LLO", t.GetCellsAsString(3, 0));

            Assert.AreEqual(" ", t.GetCellsAsString(0, 1));
            Assert.AreEqual(" W", t.GetCellsAsString(0, 2));
            Assert.AreEqual(" WO", t.GetCellsAsString(0, 3));

            Assert.AreEqual("O ", t.GetCellsAsString(1, 1));
            Assert.AreEqual("O W", t.GetCellsAsString(1, 2));
            Assert.AreEqual("LO ", t.GetCellsAsString(2, 1));
            Assert.AreEqual("LLO ", t.GetCellsAsString(3, 1));
            Assert.AreEqual("O WO", t.GetCellsAsString(1, 3));

            // Try asking more characters than available
            Assert.AreEqual(new string(Tape.Blank,1) + "HELLO", t.GetCellsAsString(6, 0));
            Assert.AreEqual(new string(Tape.Blank, 2) + "HELLO", t.GetCellsAsString(7, 0));
            Assert.AreEqual(new string(Tape.Blank, 3) + "HELLO", t.GetCellsAsString(8, 0));

            Assert.AreEqual(" WORLD" + new string(Tape.Blank, 1), t.GetCellsAsString(0, 7));
            Assert.AreEqual(" WORLD" + new string(Tape.Blank, 2), t.GetCellsAsString(0, 8));
            Assert.AreEqual(" WORLD" + new string(Tape.Blank, 3), t.GetCellsAsString(0, 9));

            Assert.AreEqual(new string(Tape.Blank, 1) + "HELLO WORLD" + new string(Tape.Blank, 1), t.GetCellsAsString(6, 7));
            Assert.AreEqual(new string(Tape.Blank, 1) + "HELLO WORLD" + new string(Tape.Blank, 2), t.GetCellsAsString(6, 8));
            Assert.AreEqual(new string(Tape.Blank, 2) + "HELLO WORLD" + new string(Tape.Blank, 1), t.GetCellsAsString(7, 7));
            Assert.AreEqual(new string(Tape.Blank, 2) + "HELLO WORLD" + new string(Tape.Blank, 2), t.GetCellsAsString(7, 8));
            Assert.AreEqual(new string(Tape.Blank, 3) + "HELLO WORLD" + new string(Tape.Blank, 2), t.GetCellsAsString(8, 8));
        }

        [TestMethod]
        [Description("Load the content of a file into the tape and then compare the tape with the file")]
        [DeploymentItem(@"Data\TapeContentTest.txt", "Data")]
        public void TestLoadFileIntoTape()
        {
            InternalTestLoadFileIntoTape(true); // Move right
            InternalTestLoadFileIntoTape(false); // Move left
        }

        [TestMethod()]
        public void TestBasicReadWrite()
        {
            var t = new Tape();
            t.CurrentSymbol = 'W';
            t.MoveRight();

            t.CurrentSymbol = 'O';
            t.MoveRight();

            t.CurrentSymbol = 'R';
            t.MoveRight();

            t.CurrentSymbol = 'L';
            t.MoveRight();

            t.CurrentSymbol = 'D';

            // Rewind
            t.MoveLeft();
            t.MoveLeft();
            t.MoveLeft();
            t.MoveLeft();
            t.MoveLeft();

            t.CurrentSymbol = ' ';
            t.MoveLeft();

            t.CurrentSymbol = 'O';
            t.MoveLeft();

            t.CurrentSymbol = 'L';
            t.MoveLeft();

            t.CurrentSymbol = 'L';
            t.MoveLeft();

            t.CurrentSymbol = 'E';
            t.MoveLeft();

            t.CurrentSymbol = 'H';

            // Read tape's content
            var output = new StringBuilder(11);
            for (int i = 1; i <= 11; i++)
            {
                output.Append(t.CurrentSymbol);
                t.MoveRight();
            }

            Assert.AreEqual("HELLO WORLD", output.ToString());
        }

        private void InternalTestLoadFileIntoTape(bool moveRight)
        {
            var t = new Tape();

            var fileContent = File.ReadAllText(Path.GetFullPath(@"Data\TapeContentTest.txt"));
            for (int i = 0; i < fileContent.Length; i++)
            {
                t.CurrentSymbol = fileContent[i];
                if (moveRight)
                    t.MoveRight();
                else
                    t.MoveLeft();
            }

            // Now the tape should be pointing to a blank cell
            Assert.AreEqual(Tape.Blank, t.CurrentSymbol);

            for (int i = 0; i < fileContent.Length; i++)
            {
                if (moveRight)
                    t.MoveLeft(); // Move to the opposite side because the tape is rewinding
                else
                    t.MoveRight();
                Assert.AreEqual(fileContent[fileContent.Length - 1 - i], t.CurrentSymbol);
            }
        }

        private void InternalTestWithoutGrowth(bool moveRight)
        {
            var t = new Tape();

            char k = (char)65;

            // As there are 26 chars in the English alphabet, make sure there are enough symbols for the test
            // as we only want to observe A-Z behavior to make debugging easier

            if (Tape.InitialSize > 26)
#pragma warning disable CS0162 // Unreachable code detected
                Assert.Inconclusive("Tape's initial size is too big");
#pragma warning restore CS0162 // Unreachable code detected

            do
            {
                Assert.AreEqual(Tape.Blank, t.CurrentSymbol);
                t.CurrentSymbol = k++;
            } while (!(moveRight ? t.MoveRight() : t.MoveLeft())); // Continue moving right until the tape grows

            // Rewind
            var n = k - 65;
            for (int i = 0; i < n; i++)
            {
                // As the tape is rewinding, we need to go to the opposite direction
                if (moveRight)
                    t.MoveLeft();
                else
                    t.MoveRight();

                Assert.AreEqual(--k, t.CurrentSymbol);
            }
        }

    }
}
