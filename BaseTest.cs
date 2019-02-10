using NUnit.Framework;
using DCasm;

namespace Tests
{
    [TestFixture]
    public class Tests
    {
        Scanner sc;
		Parser par;

        [SetUp]
        public void Setup()
        {
           
        }

        public bool isCorrectType<T>(INode node) {
            return node.GetType() == typeof(T);
        }

        public void ArithmeticAsserts(INode root, string immediateOp, string registerOp, string immediateValue) {
            Assert.Greater(root.Childrens.Count, 1);
            Assert.IsTrue(root.Childrens[0].Childrens.Count == 3);
            Assert.IsTrue(root.Childrens[1].Childrens.Count == 3);
            var firstAdd = root.Childrens[0];
            var secondAdd = root.Childrens[1];
            Assert.IsTrue(firstAdd.Value == immediateOp);
            Assert.IsTrue(isCorrectType<Register>(firstAdd.Childrens[0]));
            Assert.IsTrue(isCorrectType<Register>(firstAdd.Childrens[1]));
            Assert.IsTrue(isCorrectType<Const>(firstAdd.Childrens[2]));
            Assert.AreEqual(firstAdd.Childrens[2].Value, immediateValue);
            Assert.IsTrue(secondAdd.Value == registerOp);
            Assert.IsTrue(isCorrectType<Register>(secondAdd.Childrens[0]));
            Assert.IsTrue(isCorrectType<Register>(secondAdd.Childrens[1]));
            Assert.IsTrue(isCorrectType<Register>(secondAdd.Childrens[2]));
        }

        public void InTestSetup(string program) {
            sc = new Scanner(Utils.GenerateStreamFromString(program));
            par = new Parser(sc);
            //par.CurrentISA = new DCASM8();
            par.gen = new CodeGenerator();
            par.Parse();
            if (par.errors.count == 0) par.gen.Compile();
        }

        [Test]
        public void ImmediateLoad()
        {
            InTestSetup(@"
            program
            li $1 10
            ");
            Assert.Greater(par.gen.treeRoot.Childrens.Count, 0);
            //li must have 2 argument
            Assert.IsTrue(par.gen.treeRoot.Childrens[0].Childrens.Count == 2);
            var reg = par.gen.treeRoot.Childrens[0].Childrens[0];
            var con = par.gen.treeRoot.Childrens[0].Childrens[1];
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

            var root = par.gen.treeRoot;
            ArithmeticAsserts(root, "addi", "add", "10");
        }

        [Test]
        public void Sub() {
            InTestSetup(@"
            program
            sub $1 $2 10
            sub $3 $1 $2
            ");

            var root = par.gen.treeRoot;
            ArithmeticAsserts(root, "subi", "sub", "10");
        }

        [Test]
        public void div() {
            InTestSetup(@"
            program
            div $1 $2 10
            div $3 $1 $2
            ");

            var root = par.gen.treeRoot;
            ArithmeticAsserts(root, "divi", "div", "10");
        }

        [Test]
        public void mul() {
            InTestSetup(@"
            program
            mul $1 $2 10
            mul $3 $1 $2
            ");

            var root = par.gen.treeRoot;
            ArithmeticAsserts(root, "muli", "mul", "10");
        }

        [Test]
        public void ErrorAdd() {
            InTestSetup(@"
            program
            add 10 $1 $2
            add $1 $2 $3 $4
            ");

            var root = par.gen.treeRoot;
            //the parser stops at the first error 
            Assert.AreEqual(par.errors.count, 1);
        }
        
    }
}