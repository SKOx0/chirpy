using System.Linq;
using System.Runtime.InteropServices;
using Castle.DynamicProxy.Generators;
using EnvDTE;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zippy.Chirp.Tests {
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1 {
        Zippy.Chirp.TaskList tasks = new TaskList();

        public UnitTest1() {
            AttributesToAvoidReplicating.Add<TypeIdentifierAttribute>();
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestYuiCssEngine() {
            string code = "#test {\r\n\t color  : red; }";
            code = TestEngine<Zippy.Chirp.Engines.YuiCssEngine>("c:\\test.css", code);

            Assert.AreEqual(code, "#test{color:red;}");
        }

        [TestMethod]
        public void TestLessEngine() {
            string code = "@x:1px; #test { border: solid @x #000; }";
            code = TestEngine<Zippy.Chirp.Engines.LessEngine>("c:\\test.css", code);
            Assert.AreEqual(code, "#test {\n  border: solid 1px black;\n}\n");

            tasks.RemoveAll();
            code = "#test {\r\n\t color/**/  : red; }";
            code = TestEngine<Zippy.Chirp.Engines.LessEngine>("c:\\test.css", code);
            Assert.AreEqual(tasks.Errors.Count(), 1);
        }

        private string TestEngine<T>(string filename, string code) where T : Zippy.Chirp.Engines.TransformEngine, new() {
            var engine = new T();
            var item = GetProjectItem(filename);
            return engine.Transform(filename, code, item);
        }

        private Project GetProject() {
            var moq = new Moq.Mock<Project>();
            return moq.Object;
        }

        private ProjectItems GetProjectItems() {
            var moq = new Moq.Mock<ProjectItems>();
            moq.Setup(x => x.Parent).Returns(() => GetProjectItem("C:\\parentitem.cs"));
            return moq.Object;
        }

        private ProjectItem GetProjectItem(string filename) {
            var moq = new Moq.Mock<ProjectItem>();
            moq.Setup(x => x.get_FileNames(1)).Returns(filename);
            moq.Setup(x => x.ContainingProject).Returns(GetProject);
            moq.Setup(x => x.Collection).Returns(GetProjectItems);
            return moq.Object;
        }
    }
}
