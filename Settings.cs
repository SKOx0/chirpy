using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace Zippy.Chirp {


    /// <summary>
    /// Used by WDS add-in to save and retrieve its options from the registry.
    /// </summary>
    public class Settings {
        public static event Action Saved;

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
        public static string ChirpMSAjaxJsFile = ".msajax.js";
        public static string ChirpPartialViewFile = ".chirp.ascx";
        public static string ChirpViewFile = ".chirp.aspx";
        public static string ChirpLessFile = ".chirp.less";
        public static string ChirpMSAjaxLessFile = ".msajax.less";
        public static string ChirpHybridLessFile = ".hybrid.less";
        public static string ChirpMichaelAshLessFile = ".michaelash.less";

        //public static string ChirpLessCssFile = ".chirp.less.css";
        public static string ChirpCssFile = ".chirp.css";
        public static string ChirpMSAjaxCssFile = ".msajax.css";
        public static string ChirpHybridCssFile = ".hybrid.css";
        public static string ChirpMichaelAshCssFile = ".michaelash.css";

        public static string ChirpConfigFile = ".chirp.config";

        public static string[] AllExtensions;

        public static bool T4RunAsBuild = false;
        public static string T4RunAsBuildTemplate = string.Empty;
        public static bool SmartRunT4MVC = false;


        public static bool GoogleClosureOffline = false;
        public static string GoogleClosureJavaPath = string.Empty;
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
                    Settings.ChirpGctJsFile = Convert.ToString(regKey.GetValue("ChirpGctJsFile", ".gct.js"));
                    Settings.ChirpMSAjaxJsFile = Convert.ToString(regKey.GetValue("ChirpMSAjaxJsFile", ".msajax.js"));
                    Settings.ChirpLessFile = Convert.ToString(regKey.GetValue("ChirpLessFile", ".chirp.less"));
                    Settings.ChirpHybridLessFile = Convert.ToString(regKey.GetValue("ChirpHybridLessFile", ".hybrid.less"));
                    Settings.ChirpMichaelAshLessFile = Convert.ToString(regKey.GetValue("ChirpMichaelAshLessFile", ".michaelash.less"));
                    Settings.ChirpMSAjaxLessFile = Convert.ToString(regKey.GetValue("ChirpMSAjaxLessFile", ".msajax.less"));

                    //Settings.ChirpLessCssFile = Convert.ToString(regKey.GetValue("ChirpLessCssFile", ".chirp.less.css"));
                    Settings.ChirpCssFile = Convert.ToString(regKey.GetValue("ChirpCssFile", ".chirp.css"));
                    Settings.ChirpHybridCssFile = Convert.ToString(regKey.GetValue("ChirpHybridCssFile", ".hybrid.css"));
                    Settings.ChirpMichaelAshCssFile = Convert.ToString(regKey.GetValue("ChirpMichaelAshCssFile", ".michaelash.css"));
                    Settings.ChirpMSAjaxCssFile = Convert.ToString(regKey.GetValue("ChirpMSAjaxCssFile", ".msajax.css"));
                    Settings.ChirpConfigFile = Convert.ToString(regKey.GetValue("ChirpConfigFile", ".chirp.config"));

                    Settings.T4RunAsBuild = Convert.ToBoolean(regKey.GetValue("T4RunAsBuild", false));
                    Settings.T4RunAsBuildTemplate = Convert.ToString(regKey.GetValue("T4RunAsBuildTemplate", "T4MVC.tt,NHibernateMapping.tt"));
                    Settings.SmartRunT4MVC = Convert.ToBoolean(regKey.GetValue("SmartRunT4MVC", false));

                    Settings.GoogleClosureJavaPath = Convert.ToString(regKey.GetValue("GoogleClosureJavaPath", string.Empty));
                    Settings.GoogleClosureOffline = Convert.ToBoolean(regKey.GetValue("GoogleClosureOffline", false));

                    LoadExtensions();
                }
            } catch (Exception ex) {
                Debug.WriteLine("Chrip - failed to load: " + ex.Message);
            } finally {
                if (regKey != null) {
                    regKey.Close();
                }
            }
        }

        private static void LoadExtensions() {
            AllExtensions = new[]{
                 Settings.ChirpConfigFile , Settings.ChirpCssFile, Settings.ChirpGctJsFile , Settings.ChirpHybridCssFile, Settings.ChirpHybridLessFile , Settings.ChirpJsFile, Settings.ChirpLessFile, Settings.ChirpMichaelAshCssFile, Settings.ChirpMichaelAshLessFile, 
                 Settings.ChirpMSAjaxCssFile, Settings.ChirpMSAjaxJsFile, Settings.ChirpMSAjaxLessFile, Settings.ChirpPartialViewFile, Settings.ChirpSimpleJsFile, Settings.ChirpViewFile, Settings.ChirpWhiteSpaceJsFile, Settings.ChirpYUIJsFile 
            };
        }

        /// <summary>
        /// Saves options page settings to registry.
        /// </summary>
        public static void Save() {
            using (var regKey = Registry.CurrentUser.OpenSubKey(_regWDS, true) ?? Registry.CurrentUser.CreateSubKey(_regWDS)) {
                regKey.SetValue("ChirpCssFile", Settings.ChirpCssFile);
                regKey.SetValue("ChirpHybridCssFile", Settings.ChirpHybridCssFile);
                regKey.SetValue("ChirpMichaelAshCssFile", Settings.ChirpMichaelAshCssFile);
                regKey.SetValue("ChirpMSAjaxCssFile", Settings.ChirpMSAjaxCssFile);
                regKey.SetValue("ChirpJsFile", Settings.ChirpJsFile);
                regKey.SetValue("ChirpLessFile", Settings.ChirpLessFile);
                regKey.SetValue("ChirpHybridLessFile", Settings.ChirpHybridLessFile);
                regKey.SetValue("ChirpMichaelAshLessFile", Settings.ChirpMichaelAshLessFile);
                regKey.SetValue("ChirpMSAjaxLessFile", Settings.ChirpMSAjaxLessFile);
                // regKey.SetValue("ChirpLessCssFile", Settings.ChirpLessCssFile);
                regKey.SetValue("ChirpSimpleJsFile", Settings.ChirpSimpleJsFile);
                regKey.SetValue("ChirpWhiteSpaceJsFile", Settings.ChirpWhiteSpaceJsFile);
                regKey.SetValue("ChirpYUIJsFile", Settings.ChirpYUIJsFile);
                regKey.SetValue("ChirpGcJsFile", Settings.ChirpGctJsFile);
                regKey.SetValue("ChirpGctJsFile", Settings.ChirpGctJsFile);
                regKey.SetValue("ChirpMSAjaxJsFile", Settings.ChirpMSAjaxJsFile);
                regKey.SetValue("ChirpConfigFile", Settings.ChirpConfigFile);

                regKey.SetValue("T4RunAsBuild", Settings.T4RunAsBuild.ToString());
                regKey.SetValue("T4RunAsBuildTemplate", Settings.T4RunAsBuildTemplate.ToString());

                regKey.SetValue("SmartRunT4MVC", Settings.SmartRunT4MVC.ToString());

                regKey.SetValue("GoogleClosureJavaPath", Settings.GoogleClosureJavaPath);
                regKey.SetValue("GoogleClosureOffline", Settings.GoogleClosureOffline);

                LoadExtensions();

                if (Saved != null) Saved();
            }
        }
        #endregion
    }
}
