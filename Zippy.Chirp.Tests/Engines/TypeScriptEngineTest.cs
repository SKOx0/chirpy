using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zippy.Chirp.Tests.Engines
{
    [TestClass]
    public class TypeScriptEngineTest : BaseTest
    {

        [TestMethod]
        public void TestTypeScriptEngineCanRun()
        {
            string path = "c:\\test.ts";
            try
            {

                string code = "var user = 'Jane User';";

                System.IO.File.Delete(path);
                System.IO.File.WriteAllText(path, code);

                code = TestEngine<Zippy.Chirp.Engines.TypeScriptEngine>(path, code);
                Assert.AreEqual(0, TaskList.Instance.Errors.Count());
                Assert.AreNotEqual(null, code);
            }
            finally
            {
                System.IO.File.Delete(path);
            }
        }

        [TestMethod]
        public void TestTypeScriptEngineCanRunManyTime()
        {
            string path = "c:\\test.ts";
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    string code = "var user = 'Jane User';";

                    System.IO.File.Delete(path);
                    System.IO.File.WriteAllText(path, code);

                    code = TestEngine<Zippy.Chirp.Engines.TypeScriptEngine>(path, code);
                   
                    Assert.AreEqual(0, TaskList.Instance.Errors.Count());
                    Assert.AreNotEqual(null, code);
                }
            }
            finally
            {
                System.IO.File.Delete(path);
            }
        }

    }
}
