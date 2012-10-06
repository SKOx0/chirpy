using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zippy.Chirp.Tests.Engines
{
    [TestClass]
    public class CSSLintEngineTest : BaseTest
    {

        [TestMethod]
        public void TestCSSLintEngineCanRun()
        {
            string code = "rere {\r\n\t color  : red; }";
            var filename = System.IO.Path.GetTempFileName();
            try
            {
                System.IO.File.WriteAllText(filename, code);
                using (var csslinit = new Zippy.Chirp.Engines.CSSLintEngine())
                {
                    csslinit.Run(filename, GetProjectItem(filename));
                    Assert.AreEqual(0, TaskList.Instance.Errors.Count());
                }
            }
            finally
            {
                System.IO.File.Delete(filename);
            }
        }

        [TestMethod]
        public void TestCSSLintEngineThrowErrorInErrorList()
        {
            Settings settings = new Settings();
            settings.CssLintOptions.Ids = true;
            settings.Save();

            string code = "#test {\r\n\t color  : red; }";
            var filename = System.IO.Path.GetTempFileName();
            try
            {
                System.IO.File.WriteAllText(filename, code);
                using (var csslinit = new Zippy.Chirp.Engines.CSSLintEngine())
                {
                    csslinit.Run(filename, GetProjectItem(filename));
                    Assert.AreEqual(1, TaskList.Instance.Errors.Count());
                }
            }
            finally
            {
                System.IO.File.Delete(filename);
            }
        }
    }
}
