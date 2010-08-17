﻿using System;
using System.Net;
using System.Web;
using System.Xml;
using Yahoo.Yui.Compressor;

namespace Zippy.Chirp {
    public class GoogleClosureCompiler {

        private const string PostData = "js_code={0}&output_format=xml&output_info=errors&output_info=compiled_code&compilation_level={1}";
        private const string ApiEndpoint = "http://closure-compiler.appspot.com/compile";

        /// <summary>
        /// Compresses the specified file using Google's Closure Compiler algorithm.
        /// <remarks>
        /// The file to compress must be smaller than 200,000 bytes.
        /// </remarks>
        /// </summary>
        /// <param name="js">javascript to compiler.</param>
        /// <param name="compressMode">SIMPLE_OPTIMIZATIONS,WHITESPACE_ONLY,ADVANCED_OPTIMIZATIONS</param>
        /// <returns>A compressed version of the specified JavaScript file.</returns>
        public static string Compress(string js, ClosureCompilerCompressMode compressMode, Action<Microsoft.VisualStudio.Shell.TaskErrorCategory, string, int, int> onError) {
            if (string.IsNullOrEmpty(js)) return string.Empty;

            long size = js.Length;
            if (size < 200000) {
                //string source = File.ReadAllText(file);
                XmlDocument xml = CallApi(js, compressMode.ToString());

                //valid have server error
                XmlNodeList NodeServerError = xml.SelectNodes("//serverErrors");
                if (NodeServerError.Count > 0) {
                    string ErrorText = string.Empty;
                    foreach (XmlNode node in NodeServerError) {
                        if (!string.IsNullOrEmpty(ErrorText))
                            ErrorText += System.Environment.NewLine;
                        ErrorText += node.InnerText;
                        onError(Microsoft.VisualStudio.Shell.TaskErrorCategory.Error, "Server error : " + node.InnerText, 1, 1);
                    }
                    //throw new GoogleClosureCompilerErrorException(ErrorText);
                }

                //valid have Javascript error
                XmlNodeList NodeError = xml.SelectNodes("//errors");
                if (NodeError.Count > 0) {
                    string ErrorText = string.Empty;
                    foreach (XmlNode node in NodeError) {
                        if (!string.IsNullOrEmpty(ErrorText))
                            ErrorText += System.Environment.NewLine;

                        if (node.Attributes["lineno"] == null && node.Attributes["charno"] == null)
                            ErrorText += node.InnerText;
                        else
                            ErrorText += string.Format("type: {0} Line : {1} Char : {2} Error : {3}",
                                node.Attributes["type"] != null ? node.Attributes["type"].ToString() : string.Empty,
                                node.Attributes["lineno"] != null ? node.Attributes["lineno"].ToString() : string.Empty,
                                node.Attributes["charno"] != null ? node.Attributes["charno"].ToString() : string.Empty,
                                node.InnerText);

                        string TaskErrorText = string.Format("Type: {0} Error : {1}",
                            node.Attributes["type"] != null ? node.Attributes["type"].ToString() : "General",
                            node.InnerText);

                        onError(Microsoft.VisualStudio.Shell.TaskErrorCategory.Error,
                            TaskErrorText,
                             (node.Attributes["lineno"] != null ? node.Attributes["lineno"].ToString() : string.Empty).ToInt(1),
                             (node.Attributes["charno"] != null ? node.Attributes["charno"].ToString() : string.Empty).ToInt(1));
                    }
                    //throw new GoogleClosureCompilerErrorException(ErrorText);
                }
                return xml.SelectSingleNode("//compiledCode").InnerText;
            } else {
                //file too large (use YUI compressor)
                string exceptionMessage = "file size too large for Google: " + size.ToString("#,#");
                onError(Microsoft.VisualStudio.Shell.TaskErrorCategory.Warning, exceptionMessage, 1, 1);
                return "//" + exceptionMessage + Environment.NewLine + JavaScriptCompressor.Compress(js);
            }
        }




        /// <summary>
        /// Calls the API with the source file as post data.
        /// </summary>
        /// <param name="source">The content of the source file.</param>
        /// <returns>The Xml response from the Google API.</returns>
        private static XmlDocument CallApi(string source, string compressMode) {
            //http://code.google.com/intl/fr-CA/closure/compiler/docs/api-ref.html

            using (WebClient client = new WebClient()) {
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