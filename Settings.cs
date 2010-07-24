using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace Zippy.Chirp {


    /// <summary>
    /// Used by WDS add-in to save and retrieve its options from the registry.
    /// </summary>
    public class Settings {
        #region Private Fields

        private const string _regWDS = @"SOFTWARE\Microsoft\VisualStudio\10.0\Chirp";

        #endregion

        #region Constructors

        private Settings() {
        }

        static Settings() {

        }

        #endregion

        #region Properties

        public static string ChirpJsFile = ".chirp.js";
        public static string ChirpSimpleJsFile = ".simple.js";
        public static string ChirpWhiteSpaceJsFile = ".whitespace.js";
        public static string ChirpYUIJsFile = ".yui.js";
        public static string ChirpGctJsFile = ".gct.js";
        public static string ChirpPartialViewFile = ".chirp.ascx";
        public static string ChirpViewFile = ".chirp.aspx";
        public static string ChirpLessFile = ".chirp.less";
        public static string ChirpLessCssFile = ".chirp.less.css";
        public static string ChirpCssFile = ".chirp.css";
        public static string ChirpHybridCssFile = ".hybird.css";
        public static string ChirpMichaelAshCssFile = ".michaelash.css";
        
        public static string ChirpConfigFile = ".chirp.config";

        public static bool T4RunAsBuild = false;
        public static string T4RunAsBuildTemplate = string.Empty;
        public static bool SmartRunT4MVC = false;
        #endregion

        #region Public Methods

        /// <summary>
        /// Loads options page settings from registry.
        /// </summary>
        public static void Load() {
            RegistryKey regKey = null;
            try {
                regKey = Registry.CurrentUser.OpenSubKey(_regWDS, false);
                if (regKey != null) {
                    Settings.ChirpJsFile = Convert.ToString(regKey.GetValue("ChirpJsFile", ".chirp.js"));
                    Settings.ChirpSimpleJsFile = Convert.ToString(regKey.GetValue("ChirpSimpleJsFile", ".simple.js"));
                    Settings.ChirpWhiteSpaceJsFile = Convert.ToString(regKey.GetValue("ChirpWhiteSpaceJsFile", ".whitespace.js"));
                    Settings.ChirpYUIJsFile = Convert.ToString(regKey.GetValue("ChirpYUIJsFile", ".yui.js"));
                    Settings.ChirpGctJsFile = Convert.ToString(regKey.GetValue("ChirpGcJsFile", ".gct.js"));
                    Settings.ChirpLessFile = Convert.ToString(regKey.GetValue("ChirpLessFile", ".chirp.less"));
                    Settings.ChirpLessCssFile = Convert.ToString(regKey.GetValue("ChirpLessCssFile", ".chirp.less.css"));
                    Settings.ChirpCssFile = Convert.ToString(regKey.GetValue("ChirpCssFile", ".chirp.css"));
                    Settings.ChirpHybridCssFile = Convert.ToString(regKey.GetValue("ChirpHybridCssFile", ".hybird.css"));
                    Settings.ChirpMichaelAshCssFile = Convert.ToString(regKey.GetValue("ChirpMichaelAshCssFile", ".michaelash.css"));
                    Settings.ChirpConfigFile = Convert.ToString(regKey.GetValue("ChirpConfigFile", ".chirp.config"));

                    Settings.T4RunAsBuild = Convert.ToBoolean(regKey.GetValue("T4RunAsBuild", false));
                    Settings.T4RunAsBuildTemplate = Convert.ToString(regKey.GetValue("T4RunAsBuildTemplate", "T4MVC.tt,NHibernateMapping.tt"));
                    Settings.SmartRunT4MVC = Convert.ToBoolean(regKey.GetValue("SmartRunT4MVC", false));
                }
            } catch (Exception ex) {
                Debug.WriteLine("Chrip - failed to load: " + ex.Message);
            } finally {
                if (regKey != null) {
                    regKey.Close();
                }
            }
        }


        /// <summary>
        /// Saves options page settings to registry.
        /// </summary>
        public static void Save() {
            RegistryKey regKey = null;
            try {
                regKey = Registry.CurrentUser.OpenSubKey(_regWDS, true);
                if (regKey == null) {
                    regKey = Registry.CurrentUser.CreateSubKey(_regWDS);
                }

                regKey.SetValue("ChirpCssFile", Settings.ChirpCssFile);
                regKey.SetValue("ChirpHybridCssFile", Settings.ChirpHybridCssFile);
                regKey.SetValue("ChirpMichaelAshCssFile", Settings.ChirpMichaelAshCssFile);
                regKey.SetValue("ChirpJsFile", Settings.ChirpJsFile);
                regKey.SetValue("ChirpLessFile", Settings.ChirpLessFile);
                regKey.SetValue("ChirpLessCssFile", Settings.ChirpLessCssFile);
                regKey.SetValue("ChirpSimpleJsFile", Settings.ChirpSimpleJsFile);
                regKey.SetValue("ChirpWhiteSpaceJsFile", Settings.ChirpWhiteSpaceJsFile);
                regKey.SetValue("ChirpYUIJsFile", Settings.ChirpYUIJsFile);
                regKey.SetValue("ChirpGctJsFile", Settings.ChirpGctJsFile);
                regKey.SetValue("ChirpConfigFile", Settings.ChirpConfigFile);

                regKey.SetValue("T4RunAsBuild", Settings.T4RunAsBuild.ToString());
                regKey.SetValue("T4RunAsBuildTemplate", Settings.T4RunAsBuildTemplate.ToString());

                regKey.SetValue("SmartRunT4MVC", Settings.SmartRunT4MVC.ToString());

            } catch (Exception ex) {
                Debug.WriteLine("Chirp - failed to save: " + ex.Message);
            } finally {
                if (regKey != null) {
                    regKey.Close();
                }
            }
        }
        #endregion
    }
}
