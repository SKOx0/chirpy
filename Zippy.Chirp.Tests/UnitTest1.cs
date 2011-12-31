using System;
using System.Linq;
using System.Runtime.InteropServices;
using Castle.DynamicProxy.Generators;
using EnvDTE;
using EnvDTE80;
using Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zippy.Chirp.Engines;
using System.IO;
using System.Xml.Linq;
using Zippy.Chirp.Xml;
using System.Collections.Generic;

namespace Zippy.Chirp.Tests {
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class UnitTest1 {
		Zippy.Chirp.Chirp chirp;

		DTE2 app;
		TaskList tasks;

		public UnitTest1() {
			AttributesToAvoidReplicating.Add<TypeIdentifierAttribute>();
			chirp = new Chirp();

			app = GetApp();

			Array arr = null;
			chirp.OnConnection(app, ext_ConnectMode.ext_cm_AfterStartup, QuickMock<AddIn>(), ref  arr);
			tasks = new TaskList(app);
		}

		private static DTE2 GetApp() {
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

		private static T QuickMock<T>() where T : class {
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

		[TestMethod]
		public void TestLoadSettingFromConfig()
		{
			Settings setting = Settings.Instance();
			setting.ChirpConfigFile = ".chirp.config";
			setting.Save();

			File.WriteAllText("c:\\test.js", "alert('test');");
			string configFileName = "c:\\test.chirp.config";
			string xmlTest = "<root><Settings><Setting key=\"ChirpSimpleJsFile\" value=\"testyy.js\"></Setting></Settings></root>";
			File.WriteAllText(configFileName, xmlTest);

			Settings settingFromPath = Settings.Instance("c:\\test.js");

			Assert.IsTrue(settingFromPath.ChirpSimpleJsFile == "testyy.js");
			File.Delete(configFileName);
			File.Delete("c:\\test.js");
		}

		[TestMethod]
		public void TestLoadSettingFromConfig_KeyBadName()
		{
			Settings setting = Settings.Instance();
			setting.ChirpConfigFile = ".chirp.config";
			setting.ChirpSimpleJsFile = ".simple.js";
			setting.Save();

			File.WriteAllText("c:\\test.js", "alert('test');");
			string configFileName = "c:\\test.chirp.config";
			string xmlTest = "<root><Settings><Setting keyss=\"ChirpSimpleJsFile\" value=\"testyy.js\"></Setting></Settings></root>";
			File.WriteAllText(configFileName, xmlTest);

			Settings settingFromPath = Settings.Instance("c:\\test.js");

			Assert.IsTrue(settingFromPath.ChirpSimpleJsFile == ".simple.js");
			File.Delete(configFileName);
			File.Delete("c:\\test.js");
		}

		[TestMethod]
		public void TestLoadSettingFromConfig_ValueBadName()
		{
			Settings setting = Settings.Instance();
			setting.ChirpConfigFile = ".chirp.config";
			setting.ChirpSimpleJsFile = ".simple.js";
			setting.Save();

			File.WriteAllText("c:\\test.js", "alert('test');");
			string configFileName = "c:\\test.chirp.config";
			string xmlTest = "<root><Settings><Setting key=\"ChirpSimpleJsFile\" valuess=\"testyy.js\"></Setting></Settings></root>";
			File.WriteAllText(configFileName, xmlTest);

			Settings settingFromPath = Settings.Instance("c:\\test.js");

			Assert.IsTrue(settingFromPath.ChirpSimpleJsFile == ".simple.js");
			File.Delete(configFileName);
			File.Delete("c:\\test.js");
		}

		[TestMethod]
		public void TestLoadSettingFromConfig_ValueBadType()
		{
			Settings setting = Settings.Instance();
			setting.T4RunAsBuild = true;
			setting.Save();

			File.WriteAllText("c:\\test.js", "alert('test');");
			string configFileName = "c:\\test.chirp.config";
			string xmlTest = "<root><Settings><Setting key=\"T4RunAsBuild\" value=\"testyy.js\"></Setting></Settings></root>";
			File.WriteAllText(configFileName, xmlTest);

			Settings settingFromPath = Settings.Instance("c:\\test.js");

			Assert.IsTrue(settingFromPath.T4RunAsBuild == true);
			File.Delete(configFileName);
			File.Delete("c:\\test.js");
		}

		[TestMethod]
		public void TestLoadSettingFromConfig_SetValueBool()
		{
			Settings setting = Settings.Instance();
			setting.T4RunAsBuild = true;
			setting.Save();

			File.WriteAllText("c:\\test.js", "alert('test');");
			string configFileName = "c:\\test.chirp.config";
			string xmlTest = "<root><Settings><Setting key=\"T4RunAsBuild\" value=\"false\"></Setting></Settings></root>";
			File.WriteAllText(configFileName, xmlTest);

			Settings settingFromPath = Settings.Instance("c:\\test.js");

			Assert.IsTrue(settingFromPath.T4RunAsBuild == false);
			File.Delete(configFileName);
			File.Delete("c:\\test.js");
		}

		[TestMethod]
		public void TestConfigFileWithoutXmlNs()
		{
			File.WriteAllText("c:\\test.js", "alert('test');");
			string configFileName = "c:\\test.config";
			string xmlTest= "<root><FileGroup Name=\"scripts.combined.js\"><File Path=\"test.js\" Minify=\"false\" /></FileGroup></root>";
			File.WriteAllText(configFileName, xmlTest);

			TestActionEngine<Zippy.Chirp.Engines.ConfigEngine>(configFileName, xmlTest);
			
			Assert.IsTrue(File.Exists("c:\\scripts.combined.js"),"File group don't work");
			File.Delete(configFileName);
			File.Delete("c:\\test.js");
			File.Delete("c:\\scripts.combined.js");
		}

		[TestMethod]
		public void TestConfigFileWithXmlNs()
		{
			File.WriteAllText("c:\\test.js", "alert('test');");
			string configFileName = "c:\\test.xml";
			string xmlTest = "<root xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:ChirpyConfig\"><FileGroup Name=\"scripts.combined.js\"><File Path=\"test.js\" Minify=\"false\" /></FileGroup></root>";
			File.WriteAllText(configFileName, xmlTest);

			TestActionEngine<Zippy.Chirp.Engines.ConfigEngine>(configFileName, xmlTest);

			Assert.IsTrue(File.Exists("c:\\scripts.combined.js"), "File group don't work");
			File.Delete(configFileName);
			File.Delete("c:\\test.js");
			File.Delete("c:\\scripts.combined.js");
		}

		#region "Test CSS"
		private void YuiCssDefaultSetting()
		{
			Settings setting = Settings.Instance();
			setting.YuiCssSettings.ColumnWidth = 0;
			setting.YuiCssSettings.RemoveComments = true;
			setting.Save();
		}

		private void MsCssDefaultSetting()
		{
			Settings setting = Settings.Instance();
			setting.Save();
		}

		[TestMethod]
		public void TestYuiCssEngine() {
			this.YuiCssDefaultSetting();

			string code = "#test {\r\n\t color  : red; }";
			code = TestEngine<Zippy.Chirp.Engines.YuiCssEngine>("c:\\test.css", code);

			Assert.AreEqual(code, "#test{color:red}");
		}

		[TestMethod]
		public void TestYuiMARECssEngine() {
			this.YuiCssDefaultSetting();

			string code = "#test {\r\n\t color  : #ffffff; }";
			string output = Engines.YuiCssEngine.Minify(code, Zippy.Chirp.Xml.MinifyType.yuiMARE);

			Assert.AreEqual(output, "#test{color:#fff}");
		}

		[TestMethod]
		public void TestYuiHybridCssEngine() {
			this.YuiCssDefaultSetting();

			string code = "#test {\r\n\t color  : #ffffff; }";
			string output = Engines.YuiCssEngine.Minify(code, Zippy.Chirp.Xml.MinifyType.yuiHybrid);

			Assert.AreEqual(output, "#test{color:#fff}");
		}


		/// <summary>
		/// Test Microsoft Ajax Minifer (javascript)
		/// </summary>
		[TestMethod]
		public void TestMsCssEngine() {
			this.MsCssDefaultSetting();
			string code = "#test {\r\n\t color  : red; }";
			code = TestEngine<Zippy.Chirp.Engines.MsCssEngine>("c:\\test.css", code);

			Assert.AreEqual(code, "#test{color:red}");
		}
		#endregion

		#region "Test JS"
		private void YuiJsDefaultSetting()
		{
			Settings setting = Settings.Instance();
			setting.YuiJsSettings.DisableOptimizations = false;
			setting.YuiJsSettings.IsObfuscateJavascript = false;
			setting.YuiJsSettings.LineBreakPosition = 0;
			setting.YuiJsSettings.PreserveAllSemiColons = false;
			setting.Save();
		}

		private void MsJsDefaultSetting()
		{
			Settings setting = Settings.Instance();
			setting.Save();
		}
		#region "MsAjax"

		[TestMethod]
		public void TestJsHintEngine() {
			Settings settings = new Settings();
			settings.JsHintOptions.bitwise = true;
			settings.JsHintOptions.boss = true;
			settings.JsHintOptions.curly = true;
			settings.JsHintOptions.debug = true;
			settings.JsHintOptions.devel = true;
			settings.JsHintOptions.eqeqeq = true;
			settings.JsHintOptions.evil = true;
			settings.JsHintOptions.forin = true;
			settings.JsHintOptions.immed = true;
			settings.JsHintOptions.laxbreak = true;
			settings.JsHintOptions.maxerr = 25;
			settings.JsHintOptions.newcapp = true;
			settings.JsHintOptions.noarg = true;
			settings.JsHintOptions.noempty = true;
			settings.JsHintOptions.nomen = true;
			settings.JsHintOptions.nonew = true;
			settings.JsHintOptions.novar = true;
			settings.JsHintOptions.passfail = true;
			settings.JsHintOptions.plusplus = true;
			settings.JsHintOptions.regex = true;
			settings.JsHintOptions.strict = true;
			settings.JsHintOptions.sub = true;
			settings.JsHintOptions.undef = true;
			settings.JsHintOptions.white = true;
			settings.Save();

			string code = "if(test) {\r\n\t eval('test'); }";
			var filename = System.IO.Path.GetTempFileName();
			try {
				System.IO.File.WriteAllText(filename, code);
				using (var jshint = new Zippy.Chirp.Engines.JSHintEngine()) {
					jshint.Run(filename, GetProjectItem(filename));
					Assert.AreNotEqual(0, TaskList.Instance.Errors.Count());
				}
			} finally {
				System.IO.File.Delete(filename);
			}
		}

		[TestMethod]
		public void TestUglifyJsEngine() {
			this.YuiJsDefaultSetting();
			string code = "if(test) {\r\n\t alert('test'); }";
			code = TestEngine<Zippy.Chirp.Engines.UglifyEngine>("c:\\test.js", code);

			Assert.AreEqual(code, "test&&alert(\"test\")");
		}

		[TestMethod]
		public void TestMsJsEngine() {
			this.MsJsDefaultSetting();
			string code = "if(test) {\r\n\t alert('test'); }";
			code = TestEngine<Zippy.Chirp.Engines.MsJsEngine>("c:\\test.js", code);

			Assert.AreEqual(code, "test&&alert(\"test\")");
		}

		[TestMethod]
		public void TestMsJsEngineThrowTaskListErrorOnJsError() {
			this.MsJsDefaultSetting();
			TaskList.Instance.RemoveAll();
			string code = "if(test){;";
			code = TestEngine<Zippy.Chirp.Engines.MsJsEngine>("c:\\test.js", code);

			Assert.AreEqual(TaskList.Instance.Errors.Count(), 1);
		}
		#endregion

		#region "YuiCompressor"

		[TestMethod]
		public void TestYuiJsEngine() {
			this.YuiJsDefaultSetting();
			string code = "if(test) {\r\n\t alert('test'); }";
			code = TestEngine<Zippy.Chirp.Engines.YuiJsEngine>("c:\\test.js", code);

			Assert.IsTrue(code == "if(test){alert(\"test\")};" || code == "if(test){alert(\"test\")\n};");
		}

		[TestMethod]
		public void TestYuiJsEngineThrowTaskListErrorOnJsError() {
			this.YuiJsDefaultSetting();
			TaskList.Instance.RemoveAll();
			string code = "if(test  }";
			code = TestEngine<Zippy.Chirp.Engines.YuiJsEngine>("c:\\test.js", code);

			Assert.AreEqual(TaskList.Instance.Errors.Count(), 1);
		}
		#endregion

		#region "CoffeeScript"
		[TestMethod]
		public void TestCoffeeScriptEngine() {
			string code = "alert \"Hello CoffeeScript!\"";
			string TempFilePath = System.Environment.CurrentDirectory + "\\test.js";
			System.IO.File.WriteAllText(TempFilePath, code);

			code = TestEngine<Zippy.Chirp.Engines.CoffeeScriptEngine>(TempFilePath, code);

			Assert.AreEqual(code, "(function() {\n  alert(\"Hello CoffeeScript!\");\n}).call(this);\n");
		}
		#endregion


		#region "ClosureCompiler"

		[TestMethod]
		public void TestClosureCompilerJsEngine() {
			string code = "if(test) {\r\n\t alert('test'); }";
			//create file for googleClosureCompilerOffline
			string TempFilePath = System.Environment.CurrentDirectory + "\\test.js";
			System.IO.File.WriteAllText(TempFilePath, code);

			code = TestEngine<Zippy.Chirp.Engines.ClosureCompilerEngine>(TempFilePath, code);

			Assert.IsTrue(code == "if(test)alert(\"test\");" || code == "if(test)alert(\"test\");\r\n");
		}

		[TestMethod]
		public void TestClosureCompilerAdvancedOfflineJsEngine() {
			Settings settings = new Settings();
			settings.GoogleClosureOffline = true;
			settings.Save();

			string code = "function hello(name) {alert('Hello, ' + name);}hello('New user');";
			//create file for googleClosureCompilerOffline
			string TempFilePath = System.Environment.CurrentDirectory + "\\test.js";
			System.IO.File.WriteAllText(TempFilePath, code);


			code = Zippy.Chirp.Engines.ClosureCompilerEngine.Minify(TempFilePath, code, GetProjectItem(TempFilePath), ClosureCompilerCompressMode.ADVANCED_OPTIMIZATIONS, string.Empty);
			Assert.IsTrue(code == "alert(\"Hello, New user\");\r\n");
		}

		[TestMethod]
		public void TestClosureCompilerAdvancedOnlineJsEngine() {
			Settings settings = new Settings();
			settings.GoogleClosureOffline = false;
			settings.Save();

			string code = "function hello(name) {alert('Hello, ' + name);}hello('New user');";
			//create file for googleClosureCompilerOffline
			string TempFilePath = System.Environment.CurrentDirectory + "\\test.js";
			System.IO.File.WriteAllText(TempFilePath, code);


			code = Zippy.Chirp.Engines.ClosureCompilerEngine.Minify(TempFilePath, code, GetProjectItem(TempFilePath), ClosureCompilerCompressMode.ADVANCED_OPTIMIZATIONS, string.Empty);
			Assert.IsTrue(code == "alert(\"Hello, New user\");\r\n" || code == "alert(\"Hello, New user\");");
		}

		[TestMethod]
		public void TestClosureCompilerAdvancedOnlineDetectFileName() {
			Settings settings = new Settings();
			settings.GoogleClosureOffline = false;
			settings.ChirpJsFile = ".chirp.js";
			settings.Save();

			string code = "function hello(name) {alert('Hello, ' + name);}hello('New user');";
			//create file for googleClosureCompilerOffline
			string TempFilePath = System.Environment.CurrentDirectory + "\\test.chirp.js";
			System.IO.File.WriteAllText(TempFilePath, code);

			ClosureCompilerEngine closureCompilerEngine = new ClosureCompilerEngine();
			code = closureCompilerEngine.Transform(TempFilePath, code, GetProjectItem(TempFilePath));
			Assert.IsTrue(code == "alert(\"Hello, New user\");\r\n" || code == "alert(\"Hello, New user\");");
		}

		[TestMethod]
		public void TestClosureCompilerSimpleJsEngine() {
			string code = "if(test) {\r\n\t alert('test'); }";
			//create file for googleClosureCompilerOffline
			string TempFilePath = System.Environment.CurrentDirectory + "\\test.js";
			System.IO.File.WriteAllText(TempFilePath, code);

			code = Zippy.Chirp.Engines.ClosureCompilerEngine.Minify(TempFilePath, code, GetProjectItem(TempFilePath), ClosureCompilerCompressMode.SIMPLE_OPTIMIZATIONS, string.Empty);
			Assert.IsTrue(code == "test&&alert(\"test\");" || code == "test&&alert(\"test\");\r\n");
		}


		[TestMethod]
		public void TestClosureCompilerWhiteSpaceOnlyJsEngine() {
			string code = "if(test) {\r\n\t alert('test'); }";
			//create file for googleClosureCompilerOffline
			string TempFilePath = System.Environment.CurrentDirectory + "\\test.js";
			System.IO.File.WriteAllText(TempFilePath, code);

			code = Zippy.Chirp.Engines.ClosureCompilerEngine.Minify(TempFilePath, code, GetProjectItem(TempFilePath), ClosureCompilerCompressMode.WHITESPACE_ONLY, string.Empty);
			Assert.IsTrue(code == "if(test)alert(\"test\");" || code == "if(test)alert(\"test\");\r\n");
		}

		[TestMethod]
		public void TestClosureCompilerJsEngineThrowTaskListErrorOnJsError() {
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
		public void TestLess() {
			string code = "@x:1px; #test { border: solid @x #000; }";

			//string code = "@import 'test';";
		}

		[TestMethod]
		public void TestLessDependencies() {
			var code = @"
					@import url(test/1);
					@import url('..\test\2');
					@import url(""/test/3"");
					@import '../test/4';
					@import ""./test/5"";              
				";

			var imports = LessEngine.FindDependencies("C:\\test\\temp\\0.less", code, "C:\\test\\");
			Assert.AreEqual(@"C:\test\temp\test\1.less".ToUri(), imports.ElementAtOrDefault(0));
			Assert.AreEqual(@"C:\test\test\2.less".ToUri(), imports.ElementAtOrDefault(1));
			Assert.AreEqual(@"C:\test\test\3.less".ToUri(), imports.ElementAtOrDefault(2));
			Assert.AreEqual(@"C:\test\test\4.less".ToUri(), imports.ElementAtOrDefault(3));
			Assert.AreEqual(@"C:\test\temp\test\5.less".ToUri(), imports.ElementAtOrDefault(4));
		}

		[TestMethod]
		public void TestLessEngine() {
			string code = "@x:1px; #test { border: solid @x #000; }";
			code = TestEngine<Zippy.Chirp.Engines.LessEngine>("c:\\test.css", code);
			Assert.IsTrue(code == "#test {\n  border: solid 1px black;\n}\n" || code == "#test{border:solid 1px #000;}");

			TaskList.Instance.RemoveAll();
			code = "#test {\r\n\t color/**/  : red; }";
			code = TestEngine<Zippy.Chirp.Engines.LessEngine>("c:\\test.css", code);
			Assert.AreEqual(TaskList.Instance.Errors.Count(), 1);
		}

		[TestMethod]
		public void TestLessEngineCompress()
		{
			Settings settings = Settings.Instance();
			settings.DotLessCompress = true;
			settings.Save();
			string codeOriginal = "@x:1px; #test {\n  border: solid @x #000;\n }\n\n\n";
			string codeResult = string.Empty;
			codeResult = TestEngine<Zippy.Chirp.Engines.LessEngine>("c:\\test.css", codeOriginal);
			Assert.IsTrue(codeResult == "#test{border:solid 1px #000;}");

			settings.DotLessCompress = false;
			settings.Save();
			codeResult = TestEngine<Zippy.Chirp.Engines.LessEngine>("c:\\test.css", codeOriginal);
			Assert.IsTrue(codeResult == "#test {\n  border: solid 1px black;\n}\n");
		}


		[TestMethod]
		public void TestScssVariables()
		{
			var input = @"$blue: #3bbfce;
$margin: 16px;

.content-navigation {
  border-color: $blue;
  color:
	darken($blue, 9%);
}

.border {
  padding: $margin / 2;
  margin: $margin / 2;
  border-color: $blue;
}";

			var result=".content-navigation {\n  border-color: #3bbfce;\n  color: #2ca2af; }\n\n.border {\n  padding: 8px;\n  margin: 8px;\n  border-color: #3bbfce; }\n";

			string code = SasscompileInput("test.scss", input);
			Assert.AreEqual(result,code);
		}


		[TestMethod]
		public void TestScssSmoke()
		{
			var input = @"
			// SCSS

			.error {
			  border: 1px #f00;
			  background: #fdd;
			}
			.error.intrusion {
			  font-size: 1.3em;
			  font-weight: bold;
			}

			.badError {
			  @extend .error;
			  border-width: 3px;
			}
			";

		   string code = SasscompileInput("test.scss", input);
		   Assert.IsFalse(string.IsNullOrWhiteSpace(code));
		}

		 [TestMethod]
		public void TestSassNegativeSmoke()
		{
			var input = ".foo bar[val=\"//\"] { baz: bang; }";
			try
			{
				string code = SasscompileInput("test.sass", input);
			}
			catch (Exception e)
			{
				Assert.IsTrue(e.ToString().Contains("Syntax"));
			}
		}

		string SasscompileInput(string filename, string input)
		{
			using (var of = File.CreateText(filename))
			{
				of.Write(input);
			}
			try
			{

				// TODO: Fix this
				//     fixture.Init(TODO);
				string result = TestEngine<Zippy.Chirp.Engines.SassEngine>(filename,input);
				Console.WriteLine(result);
				return result;
			}
			finally
			{
				File.Delete(filename);
			}
		}

		private string TestEngine<T>(string filename, string code) where T : Zippy.Chirp.Engines.TransformEngine, new() {
			var engine = new T();
			var item = GetProjectItem(filename);
			return engine.Transform(filename, code, item);
		}

		private void TestActionEngine<T>(string filename, string code) where T : Zippy.Chirp.Engines.ActionEngine, new()
		{
			var engine = new T();
			var item = GetProjectItem(filename);
			engine.Run(filename, item);
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
			moq.Setup(x => x.Name).Returns(filename);
			moq.Setup(x => x.ContainingProject).Returns(GetProject);
			moq.Setup(x => x.Collection).Returns(GetProjectItems);
			return moq.Object;
		}
	}
}
