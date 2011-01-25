using System;
using System.Linq;
using System.Runtime.InteropServices;
using Castle.DynamicProxy.Generators;
using EnvDTE;
using EnvDTE80;
using Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zippy.Chirp.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        Zippy.Chirp.Chirp chirp;

        DTE2 app;

        public UnitTest1()
        {
            AttributesToAvoidReplicating.Add<TypeIdentifierAttribute>();
            chirp = new Chirp();

            app = GetApp();

            Array arr = null;
            chirp.OnConnection(app, ext_ConnectMode.ext_cm_AfterStartup, QuickMock<AddIn>(), ref  arr);
        }

        private static DTE2 GetApp()
        {
            var mockApp = new Moq.Mock<DTE2>();
            var mockEvents = new Moq.Mock<Events2>();
            mockEvents.Setup(x => x.get_DocumentEvents(null)).Returns(QuickMock<DocumentEvents>());
            mockEvents.Setup(x => x.ProjectItemsEvents).Returns(QuickMock<ProjectItemsEvents>());
            mockEvents.Setup(x => x.SolutionEvents).Returns(QuickMock<SolutionEvents>());
            mockEvents.Setup(x => x.BuildEvents).Returns(QuickMock<BuildEvents>());
            mockEvents.Setup(x => x.get_CommandEvents("{" + Guid.Empty.ToString() + "}", 0)).Returns(QuickMock<CommandEvents>());

            mockApp.Setup(x => x.Events).Returns(() => mockEvents.Object);
            return mockApp.Object;
        }

        private static T QuickMock<T>() where T : class
        {
            var mock = new Moq.Mock<T>();
            return mock.Object;
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

        #region "Test CSS"
       
        [TestMethod]
        public void TestYuiCssEngine()
        {
            string code = "#test {\r\n\t color  : red; }";
            code = TestEngine<Zippy.Chirp.Engines.YuiCssEngine>("c:\\test.css", code);

            Assert.AreEqual(code, "#test{color:red;}");
        }

        [TestMethod]
        public void TestYuiMARECssEngine()
        {
            string code = "#test {\r\n\t color  : #ffffff; }";
            string output = Engines.YuiCssEngine.Minify(code, Zippy.Chirp.Xml.MinifyType.yuiMARE);

            Assert.AreEqual(output, "#test{color:#fff}");
        }

        [TestMethod]
        public void TestYuiHybridCssEngine()
        {
            string code = "#test {\r\n\t color  : #ffffff; }";
            string output = Engines.YuiCssEngine.Minify(code, Zippy.Chirp.Xml.MinifyType.yuiHybrid);

            Assert.AreEqual(output, "#test{color:#fff}");
        }


        /// <summary>
        /// Test Microsoft Ajax Minifer (javascript)
        /// </summary>
        [TestMethod]
        public void TestMsCssEngine()
        {
            string code = "#test {\r\n\t color  : red; }";
            code = TestEngine<Zippy.Chirp.Engines.MsCssEngine>("c:\\test.css", code);

            Assert.AreEqual(code, "#test{color:red}");
        }
        #endregion

        #region "Test JS"
        #region "MsAjax"
        
        [TestMethod]
        public void TestMsJsEngine()
        {
            string code = "if(test) {\r\n\t alert('test'); }";
            code = TestEngine<Zippy.Chirp.Engines.MsJsEngine>("c:\\test.js", code);

            Assert.AreEqual(code, "test&&alert(\"test\")");
        }

        [TestMethod]
        public void TestMsJsEngineThrowTaskListErrorOnJsError()
        {
            TaskList.Instance.RemoveAll();
            string code = "if(test){;";
            code = TestEngine<Zippy.Chirp.Engines.MsJsEngine>("c:\\test.js", code);

            Assert.AreEqual(TaskList.Instance.Errors.Count(), 1);
        }
        #endregion

        #region "YuiCompressor"

        [TestMethod]
        public void TestYuiJsEngine()
        {
            string code = "if(test) {\r\n\t alert('test'); }";
            code = TestEngine<Zippy.Chirp.Engines.YuiJsEngine>("c:\\test.js", code);

            Assert.AreEqual(code, "if(test){alert(\"test\")};");
        }

        [TestMethod]
        public void TestYuiJsEngineThrowTaskListErrorOnJsError()
        {
            TaskList.Instance.RemoveAll();
            string code = "if(test  }";
            code = TestEngine<Zippy.Chirp.Engines.YuiJsEngine>("c:\\test.js", code);

            Assert.AreEqual(TaskList.Instance.Errors.Count(), 1);
        }
        #endregion

        #region "ClosureCompiler"

        [TestMethod]
        public void TestClosureCompilerJsEngine()
        {
            string code = "if(test) {\r\n\t alert('test'); }";
            //create file for googleClosureCompilerOffline
            string TempFilePath=System.Environment.CurrentDirectory +"\\test.js";
            System.IO.File.WriteAllText(TempFilePath, code);
            
            code = TestEngine<Zippy.Chirp.Engines.ClosureCompilerEngine>(TempFilePath, code);

            Assert.IsTrue(code == "if(test)alert(\"test\");" || code == "if(test)alert(\"test\");\r\n");
        }

        [TestMethod]
        public void TestClosureCompilerAdvancedJsEngine()
        {
            string code = "if(test) {\r\n\t alert('test'); }";
            //create file for googleClosureCompilerOffline
            string TempFilePath = System.Environment.CurrentDirectory + "\\test.js";
            System.IO.File.WriteAllText(TempFilePath, code);


            code = Zippy.Chirp.Engines.ClosureCompilerEngine.Minify(TempFilePath, code, GetProjectItem(TempFilePath), ClosureCompilerCompressMode.ADVANCED_OPTIMIZATIONS);
            Assert.IsTrue(code == "test&&alert(\"test\");" || code == "test&&alert(\"test\");\r\n");
        }

        [TestMethod]
        public void TestClosureCompilerSimpleJsEngine()
        {
            string code = "if(test) {\r\n\t alert('test'); }";
            //create file for googleClosureCompilerOffline
            string TempFilePath = System.Environment.CurrentDirectory + "\\test.js";
            System.IO.File.WriteAllText(TempFilePath, code);

            code = Zippy.Chirp.Engines.ClosureCompilerEngine.Minify(TempFilePath, code, GetProjectItem(TempFilePath), ClosureCompilerCompressMode.SIMPLE_OPTIMIZATIONS);
            Assert.IsTrue(code == "test&&alert(\"test\");" || code == "test&&alert(\"test\");\r\n");
        }
        [TestMethod]
        public void TestClosureCompilerWhiteSpaceOnlyJsEngine()
        {
            string code = "if(test) {\r\n\t alert('test'); }";
            //create file for googleClosureCompilerOffline
            string TempFilePath = System.Environment.CurrentDirectory + "\\test.js";
            System.IO.File.WriteAllText(TempFilePath, code);

            code = Zippy.Chirp.Engines.ClosureCompilerEngine.Minify(TempFilePath, code, GetProjectItem(TempFilePath), ClosureCompilerCompressMode.WHITESPACE_ONLY);
            Assert.IsTrue(code == "if(test)alert(\"test\");" || code == "if(test)alert(\"test\");\r\n");
        }

        [TestMethod]
        public void TestClosureCompilerJsEngineThrowTaskListErrorOnJsError()
        {
            TaskList.Instance.RemoveAll();
            string code = "if(test  }";
            //create file for googleClosureCompilerOffline
            string TempFilePath = System.Environment.CurrentDirectory + "\\test.js";
            System.IO.File.WriteAllText(TempFilePath, code);

            code = TestEngine<Zippy.Chirp.Engines.ClosureCompilerEngine>(TempFilePath, code);

            Assert.AreEqual(TaskList.Instance.Errors.Count(), 1);
        }
        #endregion
        #endregion

        [TestMethod]
        public void TestLessEngine()
        {
            string code = "@x:1px; #test { border: solid @x #000; }";
            code = TestEngine<Zippy.Chirp.Engines.LessEngine>("c:\\test.css", code);
            Assert.AreEqual(code, "#test {\n  border: solid 1px black;\n}\n");

            TaskList.Instance.RemoveAll();
            code = "#test {\r\n\t color/**/  : red; }";
            code = TestEngine<Zippy.Chirp.Engines.LessEngine>("c:\\test.css", code);
            Assert.AreEqual(TaskList.Instance.Errors.Count(), 1);
        }


        private string TestEngine<T>(string filename, string code) where T : Zippy.Chirp.Engines.TransformEngine, new()
        {
            var engine = new T();
            var item = GetProjectItem(filename);
            return engine.Transform(filename, code, item);
        }

        private Project GetProject()
        {
            var moq = new Moq.Mock<Project>();
            return moq.Object;
        }

        private ProjectItems GetProjectItems()
        {
            var moq = new Moq.Mock<ProjectItems>();
            moq.Setup(x => x.Parent).Returns(() => GetProjectItem("C:\\parentitem.cs"));
            return moq.Object;
        }

        private ProjectItem GetProjectItem(string filename)
        {
            var moq = new Moq.Mock<ProjectItem>();
            moq.Setup(x => x.get_FileNames(1)).Returns(filename);
            moq.Setup(x => x.ContainingProject).Returns(GetProject);
            moq.Setup(x => x.Collection).Returns(GetProjectItems);
            return moq.Object;
        }
    }
}
