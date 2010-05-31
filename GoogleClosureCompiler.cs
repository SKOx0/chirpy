using System;
using System.Web;
using System.Xml;
using System.IO;
using System.Net;
using System.Windows.Forms;
using Yahoo.Yui.Compressor;

namespace Zippy.Chirp
{
    public class GoogleClosureCompiler
    {

        private const string PostData = "js_code={0}&output_format=xml&output_info=compiled_code&compilation_level={1}";
        private const string ApiEndpoint = "http://closure-compiler.appspot.com/compile";

        /// <summary>
        /// Compresses the specified file using Google's Closure Compiler algorithm.
        /// <remarks>
        /// The file to compress must be smaller than 200 kilobytes.
        /// </remarks>
        /// </summary>
        /// <param name="js">javascript to compiler.</param>
        /// <param name="compressMode">SIMPLE_OPTIMIZATIONS,WHITESPACE_ONLY,ADVANCED_OPTIMIZATIONS</param>
        /// <returns>A compressed version of the specified JavaScript file.</returns>
        public static string Compress(string fileName,  string compressMode)
        {
            if (!File.Exists(fileName)) throw new Exception("File does not exist: " + fileName);

            string js = File.ReadAllText(fileName);
            if (string.IsNullOrEmpty(js)) return string.Empty;

            try
            {
                long size = new FileInfo(fileName).Length;
                if (size < 160000)
                {
                    if (string.IsNullOrEmpty(compressMode)) compressMode = "SIMPLE_OPTIMIZATIONS";

                    //string source = File.ReadAllText(file);
                    XmlDocument xml = CallApi(js, compressMode);

                    return xml.SelectSingleNode("//compiledCode").InnerText;
                }
                else
                {
                    //file too large
                    string exceptionMessage = "//file size too large for Google: " + size.ToString("#,#") + Environment.NewLine;
                    return exceptionMessage + JavaScriptCompressor.Compress(js);
                }
            }
            catch
            {
                MessageBox.Show("Translation failed.");
                return js;
            }
        }


        /// <summary>
        /// Calls the API with the source file as post data.
        /// </summary>
        /// <param name="source">The content of the source file.</param>
        /// <returns>The Xml response from the Google API.</returns>
        private static XmlDocument CallApi(string source, string compressMode)
        {
            //http://code.google.com/intl/fr-CA/closure/compiler/docs/api-ref.html

            using (WebClient client = new WebClient())
            {
                client.Headers.Add("content-type", "application/x-www-form-urlencoded");
                string data = string.Format(PostData, HttpUtility.UrlEncode(source), compressMode);
                string result = client.UploadString(ApiEndpoint, data);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(result);
                return doc;
            }
        }
    }
}
