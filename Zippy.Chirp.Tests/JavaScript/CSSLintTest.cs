using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should.Fluent;
using Zippy.Chirp.JavaScript;

namespace Zippy.Chirp.Tests.JavaScript
{
    [TestClass]
    public class CSSLintTest : BaseTest
    {

        [TestMethod]
        public void TestFatalErrorToStringJS()
        {

            string code = "@media only screen and (-webkit-min-device-pixel-ratio: 2),\n" +
            "only screen and ( min--moz-device-pixel-ratio: 2),\n" +
            "only screen and ( -o-min-device-pixel-ratio: 2/1),\n" +
            "only screen and ( min-device-pixel-ratio: 2),\n" +
            "only screen and ( min-resolution: 192dpi),\nonly screen and ( min-resolution: 2dppx)";

            var result = CSSLint.CSSLINT(code);

        }

        [TestMethod]
        public void TestFatalErrorToStringJS2()
        {

            string code = "#exposeMask{.visibility:hidden;}";
            var result = CSSLint.CSSLINT(code);
        }

        [TestMethod]
        public void TestFatalErrorToStringJS3()
        {

            string code = "#exposeMask{,visibility:hidden;}";
            var result = CSSLint.CSSLINT(code);

        }

        [TestMethod]
        public void TestCSSLintWithOptionIdsTrue()
        {
            CSSLint.options options = new CSSLint.options();
            options.Ids = true;


            var result = CSSLint.CSSLINT("#fieldset {padding-top:0;}", options);

            Console.Write(result);

            Assert.AreEqual(1, result.Length);
            if (result.Length >= 1)
            {
                result[0].message.Should().Contain("Don't use IDs in selectors.");
            }

        }

        [TestMethod]
        public void TestCSSLintWithOptionIdsTwoTimeCall()
        {
            CSSLint.options options = new CSSLint.options();
            options.Ids = true;


            var result = CSSLint.CSSLINT("#fieldset {padding-top:0;}", options);

            Assert.AreEqual(result.Length, 1);
            Assert.AreEqual("Don't use IDs in selectors.", result[0].message);

            options.Ids = false;

            var resultTimeTwo = CSSLint.CSSLINT("#fieldset {padding-top:0;}", options);
            Assert.AreEqual(0, resultTimeTwo.Length);


        }

        [TestMethod]
        public void TestCSSLintManyTimeCall()
        {

            for (int i = 0; i <= 300; i++)
            {
                var result = CSSLint.CSSLINT("body {padding-top:0;}");
                Assert.AreEqual(0, result.Length);
            }

        }

        [TestMethod]
        public void TestCSSLintWithOptionIdsFalse()
        {
            CSSLint.options options = new CSSLint.options();
            options.Ids = false;
            var result = CSSLint.CSSLINT("#fieldset {padding-top:0;}", options);

            Console.Write(result);

            Assert.AreEqual(0, result.Length);

        }


    }
}
