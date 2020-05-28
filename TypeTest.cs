using System;
using DCasm;
using NUnit.Framework;
using DCasm.Visitors;
using System.Collections.Generic;
using Tests;

namespace DCasmTest
{
    [TestFixture]
    public class TypeTest
    {

        private CodeGenerator gen;
        
        private void InTestSetup(string program) {
            //par.CurrentISA = new DCASM8();
            gen = new CodeGenerator(Utils.GenerateStreamFromString(program));
            gen.Parse();
            if (gen.ErrorCount == 0) gen.Compile();
        }

        [Test]
        public void WhileWrongType() {
            InTestSetup(@"
                program
                    li $2 1
                    li $3 10
                    
                    while sw $1 $2 1 <= 10 {
                        add $2 $2 1
                    }
            ");
            Assert.Greater(gen.ErrorCount, 0);
        }
    }
}