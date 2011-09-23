using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should.Fluent;
using Zippy.Chirp.JavaScript;

namespace Zippy.Chirp.Tests {
    [TestClass]
    public class JavaScript {

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
        public void TestCoffeeScript() {
            var code = "/*CoffeeScript: bare: true */\r\nalert 'hello!'";

            using (var coffee = new CoffeeScript())
                code = coffee.compile(code);

            code.Should().Contain("alert('hello!')");
            code.Should().Not.Contain("function");
        }

        [TestMethod]
        public void CanMinifyJQuery() {
            string minid, code = Zippy.Chirp.JavaScript.Extensibility.Instance.GetContents(new Uri("http://ajax.googleapis.com/ajax/libs/jquery/1/jquery.js"));
            minid = Uglify.squeeze_it(code);

            minid.Length.Should().Be.InRange(0, code.Length - 1);
        }

        [TestMethod]
        public void MinifyBadCode() {
            string minid, code = "----*&;lij;{lo23i41";
            try {
                minid = Uglify.squeeze_it(code);

            } catch (System.Exception ex) {

                System.Console.WriteLine(ex);
            }

        }
        [TestMethod]
        public void BeautifyBadCode() {
            string minid, code = "----*&;lij;{lo23i41";
            try {
                using (var beautify = new Beautify())
                    minid = beautify.js_beautify(code);

            } catch (System.Exception ex) {

                System.Console.WriteLine(ex);
            }

        }

        [TestMethod]
        public void TestJSHintEvalIsEvil() {
            string code = "function test(){ eval(''); }";
            JSHint.result[] result;
            using (var hint = new JSHint())
                result = hint.JSHINT(code);

            result.Should().Not.Be.Null();
            result.Length.Should().Be.InRange(1, 9999);
            result[0].reason.Should().Contain("eval is evil");
        }

        [TestMethod]
        public void TestCSSLint() {
            /*  Settings settings = new Settings();
              settings.CssLintOptions.AdjoiningClasses = true;
              settings.CssLintOptions.BoxModel=true;
              settings.CssLintOptions.CompatibleVendorPrefixes = true;
              settings.CssLintOptions.DisplayPropertyGrouping = true;
              settings.CssLintOptions.DuplicateProperties = true;
              settings.CssLintOptions.EmptyRules = true;
              settings.CssLintOptions.Floats = true;
              settings.CssLintOptions.FontFaces = true;
              settings.CssLintOptions.FontSizes = true;
              settings.CssLintOptions.Gradients = true;
              settings.CssLintOptions.Ids = true;
              settings.CssLintOptions.Import = true;
              settings.CssLintOptions.Important = true;
              settings.CssLintOptions.QualifiedHeadings = true;
              settings.CssLintOptions.RegexSelectors = true;
              settings.CssLintOptions.UniqueHeadings = true;
              settings.CssLintOptions.VendorPrefix = true;
              settings.CssLintOptions.ZeroUnits = true;
              settings.Save();*/

            using (var csslint = new CSSLint()) {
                var result = csslint.CSSLINT(@"body {text-align:center;}
.container {text-align:left;}
* html .column, * html .span-1, * html .span-2, * html .span-3, * html .span-4, * html .span-5, * html .span-6, * html .span-7, * html .span-8, * html .span-9, * html .span-10, * html .span-11, * html .span-12, * html .span-13, * html .span-14, * html .span-15, * html .span-16, * html .span-17, * html .span-18, * html .span-19, * html .span-20, * html .span-21, * html .span-22, * html .span-23, * html .span-24 {display:inline;overflow-x:hidden;}
* html legend {margin:0px -8px 16px 0;padding:0;}
sup {vertical-align:text-top;}
sub {vertical-align:text-bottom;}
html>body p code {*white-space:normal;}
hr {margin:-8px auto 11px;}
img {-ms-interpolation-mode:bicubic;}
.clearfix, .container {display:inline-block;}
* html .clearfix, * html .container {height:1%;}
fieldset {padding-top:0;}
legend {margin-top:-0.2em;margin-bottom:1em;margin-left:-0.5em;}
textarea {overflow:auto;}
label {vertical-align:middle;position:relative;top:-0.25em;}
input.text, input.title, textarea {background-color:#fff;border:1px solid #bbb;}
input.text:focus, input.title:focus {border-color:#666;}
input.text, input.title, textarea, select {margin:0.5em 0;}
input.checkbox, input.radio {position:relative;top:.25em;}
form.inline div, form.inline p {vertical-align:middle;}
form.inline input.checkbox, form.inline input.radio, form.inline input.button, form.inline button {margin:0.5em 0;}
button, input.button {position:relative;top:0.25em;}");

                Console.Write(result);

                csslint.CSSLINT(".test { }").messages.Length.Should().Equal(1);
                csslint.CSSLINT(".test { color: red; }").messages.Length.Should().Equal(0);
            }
        }

        [TestMethod]
        public void TestCSSLintWithOptionIdsTrue() {
            CSSLint.options options = new CSSLint.options();
            options.Ids = true;

            using (var csslint = new CSSLint()) {
                var result = csslint.CSSLINT("#fieldset {padding-top:0;}", options);

                Console.Write(result);

                Assert.AreEqual(1, result.messages.Length);
                result.messages[0].message.Should().Contain("Don't use IDs in selectors.");

            }

        }

        [TestMethod]
        public void TestCSSLintWithOptionIdsTwoTimeCall() {
            CSSLint.options options = new CSSLint.options();
            options.Ids = true;

            using (var csslint = new CSSLint()) {
                var result = csslint.CSSLINT("#fieldset {padding-top:0;}", options);

                Assert.AreEqual(result.messages.Length, 1);
                Assert.AreEqual("Don't use IDs in selectors.", result.messages[0].message);

                options.Ids = false;

                var resultTimeTwo = csslint.CSSLINT("#fieldset {padding-top:0;}", options);
                Assert.AreEqual(0, resultTimeTwo.messages.Length);
            }

        }

        [TestMethod]
        public void TestCSSLintWithOptionIdsFalse() {
            CSSLint.options options = new CSSLint.options();
            options.Ids = false;

            using (var csslint = new CSSLint()) {
                var result = csslint.CSSLINT("#fieldset {padding-top:0;}", options);

                Console.Write(result);

                Assert.AreEqual(0, result.messages.Length);
            }

        }

        [TestMethod]
        public void TestJSHintOK() {
            string code = "function test(){ }";
            JSHint.result[] result;
            using (var hint = new JSHint())
                result = hint.JSHINT(code);

            result.Should().Be.Null();
        }

        [TestMethod]
        public void TestJSHintOptions() {
            string code = "function test(){ if(true) return (/./).test(''); }";
            JSHint.result[] result;
            using (var hint = new JSHint())
                result = hint.JSHINT(code, new JSHint.options { regex = true, curly = true });
            result.Should().Not.Be.Null();
            result.Length.Should().Be.InRange(1, 9999);
            result[0].reason.Should().Contain("Expected '{'");
        }

        [TestMethod]
        public void CanBeautify() {
            string minid, code = "if(test==0){alert(1);}";
            using (var beautify = new Beautify())
                minid = beautify.js_beautify(code);

            minid.Length.Should().Be.InRange(code.Length, int.MaxValue);
        }

        [TestMethod]
        public void CanRunMultipleTimes() {

            for (var j = 0; j < 10; j++) {
                var times = new List<long>();
                var stopwatch = new System.Diagnostics.Stopwatch();

                for (var i = 0; i < 10; i++) {
                    stopwatch.Start();

                    string code = "if(test==0){alert(1);}";
                    string minid = Uglify.squeeze_it(code);

                    minid.Length.Should().Be.InRange(1, code.Length - 1);
                    stopwatch.Stop();
                    times.Add(stopwatch.ElapsedMilliseconds);
                    stopwatch.Reset();
                }

                Console.WriteLine(string.Join(", ", times));
            }
        }

        [TestMethod]
        public void TestUglify() {

            for (var i = 0; i < 10; i++) {
                string code = "\"use strict\"; if (test == 0) { alert(1); }";
                string minid = Uglify.squeeze_it(code);

                minid.Length.Should().Be.InRange(1, code.Length - 1);
            }

        }

        [TestMethod]
        public void CanRunMultipleTimes_JSHINT() {
            for (var j = 0; j < 10; j++)
                using (var engine = new JSHint()) {
                    for (var i = 0; i < 10; i++) {
                        string code = "if(test==0){alert(1);}";
                        engine.JSHINT(code);
                    }
                }
        }

    }
}
