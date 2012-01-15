using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zippy.Chirp.Tests.Engines
{
    [TestClass]
    public class YuiJsEngineTest:BaseTest
    {
        private void YuiJsDefaultSetting()
        {
            Settings setting = Settings.Instance();
            setting.YuiJsSettings.DisableOptimizations = false;
            setting.YuiJsSettings.IsObfuscateJavascript = false;
            setting.YuiJsSettings.LineBreakPosition = 0;
            setting.YuiJsSettings.PreserveAllSemiColons = false;
            setting.Save();
        }

        [TestMethod]
        public void TestYuiJsEngine()
        {
            this.YuiJsDefaultSetting();
            string code = "if(test) {\r\n\t alert('test'); }";
            code = TestEngine<Zippy.Chirp.Engines.YuiJsEngine>("c:\\test.js", code);

            Assert.IsTrue(code == "if(test){alert(\"test\")};" || code == "if(test){alert(\"test\")\n};");
        }

        [TestMethod]
        public void TestYuiJsEngineThrowTaskListErrorOnJsError()
        {
            this.YuiJsDefaultSetting();
            TaskList.Instance.RemoveAll();
            string code = "if(test  }";
            code = TestEngine<Zippy.Chirp.Engines.YuiJsEngine>("c:\\test.js", code);

            Assert.AreEqual(TaskList.Instance.Errors.Count(), 1);
        }
    }
}
