using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace DCasmTest
{
    [TestFixture]
    public class FileGenerationTest
    {
        public byte[] HexStringToByte(string hex) => Enumerable.Range(0, hex.Length / 2) .Select(x => Convert.ToByte(hex.Substring(x * 2, 2), 16)) .ToArray();

        public byte[] HexStringToByte(IEnumerable<string> hexArray) {
            var hex = hexArray.Aggregate((string acc, string next) => acc += next);
            return Enumerable.Range(0, hex.Length / 2) .Select(x => Convert.ToByte(hex.Substring(x * 2, 2), 16)) .ToArray();
        }

        [Test]
        public void WriteOneInstructionHexa() {
            var result = HexStringToByte("AD");
            Assert.AreEqual(result[0], 173);
        }

        [Test]
        public void WriteMultipleInstructionsHexa() {
            var result = HexStringToByte(new [] {"AD", "AF"});
            Assert.AreEqual(173, result[0]);
            Assert.AreEqual(175, result[1]);
        }
    }
}