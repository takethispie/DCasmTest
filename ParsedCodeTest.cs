using NUnit.Framework;
using DCasm;
using System.Collections.Generic;

namespace Tests
{
    [TestFixture]
    public class ParsedCodeTest
    {
        CodeGenerator gen;

        [SetUp]
        public void Setup()
        {
           
        }

        public bool isCorrectType<T>(INode node) {
            return node.GetType() == typeof(T);
        }

        public void ArithmeticAsserts(List<INode> root, string immediateOp, string registerOp, string immediateValue) {
            Assert.Greater(root.Count, 1);
            Assert.IsTrue(root[0].Children.Count == 3);
            Assert.IsTrue(root[1].Children.Count == 3);
            var firstAdd = root[0];
            var secondAdd = root[1];
            Assert.IsTrue(firstAdd.Value == immediateOp);
            Assert.IsTrue(isCorrectType<Register>(firstAdd.Children[0]));
            Assert.IsTrue(isCorrectType<Register>(firstAdd.Children[1]));
            Assert.IsTrue(isCorrectType<Const>(firstAdd.Children[2]));
            Assert.AreEqual(firstAdd.Children[2].Value, immediateValue);
            Assert.IsTrue(secondAdd.Value == registerOp);
            Assert.IsTrue(isCorrectType<Register>(secondAdd.Children[0]));
            Assert.IsTrue(isCorrectType<Register>(secondAdd.Children[1]));
            Assert.IsTrue(isCorrectType<Register>(secondAdd.Children[2]));
        }

        public void InTestSetup(string program) {
            //par.CurrentISA = new DCASM8();
            gen = new CodeGenerator(Utils.GenerateStreamFromString(program));
            gen.Parse();
            if (gen.ErrorCount == 0) gen.Compile();
        }

        [Test]
        public void ImmediateLoad()
        {
            InTestSetup(@"
            program
            li $1 10
            ");
            Assert.Greater(gen.RootNodes.Count, 0);
            //li must have 2 argument
            Assert.IsTrue(gen.RootNodes[0].Children.Count == 2);
            var reg = gen.RootNodes[0].Children[0];
            var con = gen.RootNodes[0].Children[1];
            Assert.IsTrue(reg.GetType() == typeof(Register));
            Assert.IsTrue(reg.Value == "$1");
            Assert.IsTrue(con.GetType() == typeof(Const));
            Assert.IsTrue(con.Value == "10");
        }

        [Test]
        public void Add() {
            InTestSetup(@"
            program
            add $1 $2 10
            add $3 $1 $2
            ");

            var root = gen.RootNodes;
            ArithmeticAsserts(root, "addi", "add", "10");
        }

        [Test]
        public void Sub() {
            InTestSetup(@"
            program
            sub $1 $2 10
            sub $3 $1 $2
            ");

            var root = gen.RootNodes;
            ArithmeticAsserts(root, "subi", "sub", "10");
        }

        [Test]
        public void div() {
            InTestSetup(@"
            program
            div $1 $2 10
            div $3 $1 $2
            ");

            var root = gen.RootNodes;
            ArithmeticAsserts(root, "divi", "div", "10");
        }

        [Test]
        public void mul() {
            InTestSetup(@"
            program
            mul $1 $2 10
            mul $3 $1 $2
            ");

            var root = gen.RootNodes;
            ArithmeticAsserts(root, "muli", "mul", "10");
        }

        [Test]
        public void ErrorAddArgOrder() {
            InTestSetup(@"
            program
            add 10 $1 $2
            ");

            var root = gen.RootNodes;
            //the parser stops at the first error 
            Assert.AreEqual(gen.ErrorCount, 1);
        }

        [Test]
        public void ErrorAddArgNum() {
            InTestSetup(@"
            program 
            {
                add $1 $2 $3 $4
            }
            ");

            var root = gen.RootNodes;
            //the parser stops at the first error 
            Assert.AreEqual(gen.ErrorCount, 1);
        }

        [Test]
        public void Store() {
            InTestSetup(@"
            program
            sw $1 $2 1
            ");
            var root = gen.RootNodes;
            Assert.AreEqual(root.Count, 1);
            var store = root[0];

            Assert.AreEqual(store.Children.Count, 3);
            Assert.AreEqual(store.Children[0].Value, "$1");
            Assert.AreEqual(store.Children[1].Value, "$2");
            Assert.AreEqual(store.Children[2].Value, "1");
        }
        
    }
}