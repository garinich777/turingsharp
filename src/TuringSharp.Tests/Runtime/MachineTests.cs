using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSharp.CodeDom;
using TuringSharp.Parser;
using TuringSharp.Runtime;

namespace TuringSharp.Tests.Runtime
{
    [TestClass]
    public class MachineTests
    {

        [TestMethod()]
        [DeploymentItem(@"Data\binaryaddition.txt", "Data")]
        public void TestBinaryAdditionHalts()
        {
            var machine = new Machine();
            machine.Load(new ProgramParser().Parse(File.ReadAllText(@"Data\binaryaddition.txt")), "110110_101011");

            machine.Run();
            Assert.AreEqual(true, machine.IsHalted);
        }

        [TestMethod()]
        [DeploymentItem(@"Data\binaryaddition.txt", "Data")]
        public void TestBinaryAdditionWorks()
        {
            var machine = new Machine();
            machine.Load(new ProgramParser().Parse(File.ReadAllText(@"Data\binaryaddition.txt")), "110110_101011");

            machine.Run();
            Assert.AreEqual("1100001", machine.Tape.GetCellsAsString(0, 7));

            machine.Reset("110110_101010");
            machine.Run();
            Assert.AreEqual("1100000", machine.Tape.GetCellsAsString(0, 7));

            machine.Reset("110_010");
            machine.Run();
            Assert.AreEqual("1000", machine.Tape.GetCellsAsString(0, 4));
        }

        [TestMethod()]
        [DeploymentItem(@"Data\palindromedetector.txt", "Data")]
        public void TestPalindromeDetector()
        {
            var machine = new Machine();
            machine.Load(new ProgramParser().Parse(File.ReadAllText(@"Data\palindromedetector.txt")), null);

            var successTest = new List<string>() { "1001001", "1", "11", "000", "010", "0110", "01010", "1110111" };
            var failTest = new List<string>() { "1001011", "01", "10", "001", "100", "0101", "01111", "0001010" };

            foreach(var test in successTest)
            {
                machine.Reset(test);
                machine.Run();
                Assert.AreEqual(":)", machine.Tape.GetCellsAsString(1, 1), "Expected success with this test: {0}", test);
            }

            foreach (var test in failTest)
            {
                machine.Reset(test);
                machine.Run();
                Assert.AreEqual(":(", machine.Tape.GetCellsAsString(1, 1), "Expected fail with this test: {0}", test);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(IncompleteProgramException), "An empty program should throw an exception")]
        public void TestEmptyProgramWithEmptyTape()
        {
            var machine = new Machine();
            var program = new Program();

            machine.Load(program, null);
            machine.Run();
        }


    }
}
