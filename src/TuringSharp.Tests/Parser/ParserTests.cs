using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using TuringSharp.Parser;

namespace TuringSharp.Tests.Parser
{
    [TestClass]
    public class ParserTests
    {


        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get;
            set;
        }

        [TestMethod]
        [DeploymentItem(@"Data\perfectlyformatted.txt", "Data")]
        public void TestParsePerfectlyFormattedProgram()
        {
            var testData = File.ReadAllText(@"Data\perfectlyformatted.txt");
            var reader = new ProgramParser();
            var program = reader.Parse(testData);

            Assert.AreEqual(22, program.Statements.Count, "The number of parsed statements is not equal to the number statements in the file");
        }

        /// <summary>
        /// Checks the parser's compatibility with Anthony Morphett's TM simulator (http://morphett.info/turing/turing.html)
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\binaryaddition.txt", "Data")]
        public void TestCompatibilityWithMorphettsSimulator()
        {
            var testData = File.ReadAllText(@"Data\binaryaddition.txt");
            var reader = new ProgramParser();
            var program = reader.Parse(testData);

            Assert.AreEqual(29, program.Statements.Count, "The number of parsed statements is not equal to the number statements in the file");
        }

    }
}
