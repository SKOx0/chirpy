using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace Zippy.Chirp
{
    /// <summary>
    /// Used by WDS add-in to save and retrieve its options from the registry.
    /// </summary>
    public class Settings 
    {
        #region Private Fields
        private const string RegWDS = @"SOFTWARE\Microsoft\VisualStudio\10.0\Chirp";
        private static string chirpJsFile = ".chirp.js";
        private static string chirpSimpleJsFile = ".simple.js";
        private static string chirpWhiteSpaceJsFile = ".whitespace.js";
        private static string chirpYUIJsFile = ".yui.js";
        private static string chirpGctJsFile = ".gct.js";
        private static string chirpMSAjaxJsFile = ".msajax.js";
        private static string chirpPartialViewFile = ".chirp.ascx";
        private static string chirpMichaelAshLessFile = ".michaelash.less";
        private static string chirpViewFile = ".chirp.aspx";
        private static string chirpRazorCSViewFile = ".chirp.cshtml";
        private static string chirpRazorVBViewFile = ".chirp.vbhtml";
        private static string chirpMSAjaxLessFile = ".msajax.less";
        private static string chirpLessFile = ".chirp.less";
        private static string chirpHybridLessFile = ".hybrid.less";
        private static string chirpCoffeeScriptFile = ".chirp.coffee";
        private static string chirpUglifyJsFile = ".uglify.js";
        private static string chirpSimpleCoffeeScriptFile = ".simple.coffee";
        private static string chirpWhiteSpaceCoffeeScriptFile = ".whitespace.coffee";
        private static string chirpYUICoffeeScriptFile = ".yui.coffee";
        private static string chirpGctCoffeeScriptFile = ".gct.coffee";
        private static string coffeeScriptBatFilePath = string.Empty;
        private static string chirpCssFile = ".chirp.css";
        private static string chirpMSAjaxCssFile = ".msajax.css";
        private static string chirpHybridCssFile = ".hybrid.css";
        private static string chirpMichaelAshCssFile = ".michaelash.css";
        private static bool t4RunAsBuild = false;
        private static string t4RunAsBuildTemplate = string.Empty;
        private static bool smartRunT4MVC = false;
        private static bool runJSHint = true;
        private static bool googleClosureOffline = false;
        private static string googleClosureJavaPath = string.Empty;
        private static string chirpMSAjaxCoffeeScriptFile = ".msajax.coffee";
        private static string chirpUglifyCoffeeScriptFile = ".uglify.coffee";
        private static string chirpConfigFile = ".chirp.config";
        private static bool showDetailLog = true;
        private static string[] allExtensions;
        private static Xml.MinifyType defaultCssMinifier = Xml.MinifyType.yui;
        private static Xml.MinifyType defaultJavaScriptMinifier = Xml.MinifyType.yui;
        #endregion

        #region Constructors
        static Settings()
        {
        }

        private Settings()
        {
        }
        #endregion

        public static event Action Saved;

        #region Properties
        public static string ChirpJsFile
        {
            get { return Settings.chirpJsFile; }
            set { Settings.chirpJsFile = value; }
        }
      
        public static string ChirpSimpleJsFile
        {
            get { return Settings.chirpSimpleJsFile; }
            set { Settings.chirpSimpleJsFile = value; }
        }
      
        public static string ChirpWhiteSpaceJsFile
        {
            get { return Settings.chirpWhiteSpaceJsFile; }
            set { Settings.chirpWhiteSpaceJsFile = value; }
        }

        public static string ChirpYUIJsFile
        {
            get { return Settings.chirpYUIJsFile; }
            set { Settings.chirpYUIJsFile = value; }
        }
       
        public static string ChirpGctJsFile
        {
            get { return Settings.chirpGctJsFile; }
            set { Settings.chirpGctJsFile = value; }
        }
       
        public static string ChirpMSAjaxJsFile
        {
            get { return Settings.chirpMSAjaxJsFile; }
            set { Settings.chirpMSAjaxJsFile = value; }
        }
        
        public static string ChirpPartialViewFile
        {
            get { return Settings.chirpPartialViewFile; }
            set { Settings.chirpPartialViewFile = value; }
        }
      
        public static string ChirpViewFile
        {
            get { return Settings.chirpViewFile; }
            set { Settings.chirpViewFile = value; }
        }
     
        public static string ChirpRazorCSViewFile
        {
            get { return Settings.chirpRazorCSViewFile; }
            set { Settings.chirpRazorCSViewFile = value; }
        }
       
        public static string ChirpRazorVBViewFile
        {
            get { return Settings.chirpRazorVBViewFile; }
            set { Settings.chirpRazorVBViewFile = value; }
        }
       
        public static string ChirpLessFile
        {
            get { return Settings.chirpLessFile; }
            set { Settings.chirpLessFile = value; }
        }
       
        public static string ChirpMSAjaxLessFile
        {
            get { return Settings.chirpMSAjaxLessFile; }
            set { Settings.chirpMSAjaxLessFile = value; }
        }
      
        public static string ChirpHybridLessFile
        {
            get { return Settings.chirpHybridLessFile; }
            set { Settings.chirpHybridLessFile = value; }
        }
        
        public static string ChirpMichaelAshLessFile
        {
            get { return Settings.chirpMichaelAshLessFile; }
            set { Settings.chirpMichaelAshLessFile = value; }
        }
       
        public static string ChirpUglifyJsFile
        {
            get { return Settings.chirpUglifyJsFile; }
            set { Settings.chirpUglifyJsFile = value; }
        }

        public static string ChirpCoffeeScriptFile
        {
            get { return Settings.chirpCoffeeScriptFile; }
            set { Settings.chirpCoffeeScriptFile = value; }
        }
       
        public static string ChirpSimpleCoffeeScriptFile
        {
            get { return Settings.chirpSimpleCoffeeScriptFile; }
            set { Settings.chirpSimpleCoffeeScriptFile = value; }
        }
        
        public static string ChirpWhiteSpaceCoffeeScriptFile
        {
            get { return Settings.chirpWhiteSpaceCoffeeScriptFile; }
            set { Settings.chirpWhiteSpaceCoffeeScriptFile = value; }
        }
        
        public static string ChirpYUICoffeeScriptFile
        {
            get { return Settings.chirpYUICoffeeScriptFile; }
            set { Settings.chirpYUICoffeeScriptFile = value; }
        }
       
        public static string ChirpGctCoffeeScriptFile
        {
            get { return Settings.chirpGctCoffeeScriptFile; }
            set { Settings.chirpGctCoffeeScriptFile = value; }
        }
       
        public static string ChirpMSAjaxCoffeeScriptFile
        {
            get { return Settings.chirpMSAjaxCoffeeScriptFile; }
            set { Settings.chirpMSAjaxCoffeeScriptFile = value; }
        }
       
        public static string ChirpUglifyCoffeeScriptFile
        {
            get { return Settings.chirpUglifyCoffeeScriptFile; }
            set { Settings.chirpUglifyCoffeeScriptFile = value; }
        }
        
        public static string CoffeeScriptBatFilePath
        {
            get { return Settings.coffeeScriptBatFilePath; }
            set { Settings.coffeeScriptBatFilePath = value; }
        }

        public static string ChirpCssFile
        {
            get { return Settings.chirpCssFile; }
            set { Settings.chirpCssFile = value; }
        }
        
        public static string ChirpMSAjaxCssFile
        {
            get { return Settings.chirpMSAjaxCssFile; }
            set { Settings.chirpMSAjaxCssFile = value; }
        }
        
        public static string ChirpHybridCssFile
        {
            get { return Settings.chirpHybridCssFile; }
            set { Settings.chirpHybridCssFile = value; }
        }
        
        public static string ChirpMichaelAshCssFile
        {
            get { return Settings.chirpMichaelAshCssFile; }
            set { Settings.chirpMichaelAshCssFile = value; }
        }

        public static string ChirpConfigFile
        {
            get { return Settings.chirpConfigFile; }
            set { Settings.chirpConfigFile = value; }
        }

        public static Xml.MinifyType DefaultCssMinifier
        {
            get { return Settings.defaultCssMinifier; }
            set { Settings.defaultCssMinifier = value; }
        }
        
        public static Xml.MinifyType DefaultJavaScriptMinifier
        {
            get { return Settings.defaultJavaScriptMinifier; }
            set { Settings.defaultJavaScriptMinifier = value; }
        }

        public static string[] AllExtensions
        {
            get { return Settings.allExtensions; }
            set { Settings.allExtensions = value; }
        }

        public static bool T4RunAsBuild
        {
            get { return Settings.t4RunAsBuild; }
            set { Settings.t4RunAsBuild = value; }
        }
        
        public static string T4RunAsBuildTemplate
        {
            get { return Settings.t4RunAsBuildTemplate; }
            set { Settings.t4RunAsBuildTemplate = value; }
        }
        
        public static bool SmartRunT4MVC
        {
            get { return Settings.smartRunT4MVC; }
            set { Settings.smartRunT4MVC = value; }
        }
        
        public static bool RunJSHint
        {
            get { return Settings.runJSHint; }
            set { Settings.runJSHint = value; }
        }
        
        public static bool GoogleClosureOffline
        {
            get { return Settings.googleClosureOffline; }
            set { Settings.googleClosureOffline = value; }
        }
        
        public static string GoogleClosureJavaPath
        {
            get { return Settings.googleClosureJavaPath; }
            set { Settings.googleClosureJavaPath = value; }
        }

        public static bool ShowDetailLog 
        { 
            get{ return Settings.showDetailLog; } 
            set{ Settings.showDetailLog=value; } 
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Loads options page settings from registry.
        /// </summary>
        public static void Load() 
        {
            RegistryKey regKey = null;
            try 
            {
                regKey = Registry.CurrentUser.OpenSubKey(RegWDS, false);
                if (regKey != null)
                {
                    Settings.ChirpJsFile = Convert.ToString(regKey.GetValue("ChirpJsFile", ".chirp.js"));
                    Settings.ChirpSimpleJsFile = Convert.ToString(regKey.GetValue("ChirpSimpleJsFile", ".simple.js"));
                    Settings.ChirpWhiteSpaceJsFile = Convert.ToString(regKey.GetValue("ChirpWhiteSpaceJsFile", ".whitespace.js"));
                    Settings.ChirpYUIJsFile = Convert.ToString(regKey.GetValue("ChirpYUIJsFile", ".yui.js"));
                    Settings.ChirpGctJsFile = Convert.ToString(regKey.GetValue("ChirpGcJsFile", ".gct.js"));
                    Settings.ChirpMSAjaxJsFile = Convert.ToString(regKey.GetValue("ChirpMSAjaxJsFile", ".msajax.js"));
                    Settings.ChirpLessFile = Convert.ToString(regKey.GetValue("ChirpLessFile", ".chirp.less"));
                    Settings.ChirpHybridLessFile = Convert.ToString(regKey.GetValue("ChirpHybridLessFile", ".hybrid.less"));
                    Settings.ChirpMichaelAshLessFile = Convert.ToString(regKey.GetValue("ChirpMichaelAshLessFile", ".michaelash.less"));
                    Settings.ChirpMSAjaxLessFile = Convert.ToString(regKey.GetValue("ChirpMSAjaxLessFile", ".msajax.less"));
                    Settings.ChirpUglifyJsFile = Convert.ToString(regKey.GetValue("ChirpUglifyJsFile", ".uglify.js"));

                    Settings.ChirpCoffeeScriptFile = Convert.ToString(regKey.GetValue("ChirpCoffeeScriptFile", ".chirp.coffee"));
                    Settings.ChirpSimpleCoffeeScriptFile = Convert.ToString(regKey.GetValue("ChirpSimpleCoffeeScriptFile", ".simple.coffee"));
                    Settings.ChirpWhiteSpaceCoffeeScriptFile = Convert.ToString(regKey.GetValue("ChirpWhiteSpaceCoffeeScriptFile", ".whitespace.coffee"));
                    Settings.ChirpYUICoffeeScriptFile = Convert.ToString(regKey.GetValue("ChirpYUICoffeeScriptFile", ".yui.coffee"));
                    Settings.ChirpGctCoffeeScriptFile = Convert.ToString(regKey.GetValue("ChirpGcCoffeeScriptFile", ".gct.coffee"));
                    Settings.ChirpMSAjaxCoffeeScriptFile = Convert.ToString(regKey.GetValue("ChirpMSAjaxCoffeeScriptFile", ".msajax.coffee"));
                    Settings.ChirpUglifyCoffeeScriptFile = Convert.ToString(regKey.GetValue("ChirpUglifyCoffeeScriptFile", ".uglify.coffee"));
                    Settings.CoffeeScriptBatFilePath = Convert.ToString(regKey.GetValue("CoffeeScriptBatFilePath", string.Empty));

                    // Settings.ChirpLessCssFile = Convert.ToString(regKey.GetValue("ChirpLessCssFile", ".chirp.less.css"));
                    Settings.ChirpCssFile = Convert.ToString(regKey.GetValue("ChirpCssFile", ".chirp.css"));
                    Settings.ChirpHybridCssFile = Convert.ToString(regKey.GetValue("ChirpHybridCssFile", ".hybrid.css"));
                    Settings.ChirpMichaelAshCssFile = Convert.ToString(regKey.GetValue("ChirpMichaelAshCssFile", ".michaelash.css"));
                    Settings.ChirpMSAjaxCssFile = Convert.ToString(regKey.GetValue("ChirpMSAjaxCssFile", ".msajax.css"));
                    Settings.ChirpConfigFile = Convert.ToString(regKey.GetValue("ChirpConfigFile", ".chirp.config"));
                    Settings.DefaultCssMinifier = Convert.ToString(regKey.GetValue("DefaultCssMinifier", string.Empty)).ToEnum(Xml.MinifyType.yui);
                    Settings.DefaultJavaScriptMinifier = Convert.ToString(regKey.GetValue("DefaultJavaScriptMinifier", string.Empty)).ToEnum(Xml.MinifyType.yui);

                    Settings.T4RunAsBuild = Convert.ToBoolean(regKey.GetValue("T4RunAsBuild", false));
                    Settings.T4RunAsBuildTemplate = Convert.ToString(regKey.GetValue("T4RunAsBuildTemplate", "T4MVC.tt,NHibernateMapping.tt"));
                    Settings.SmartRunT4MVC = Convert.ToBoolean(regKey.GetValue("SmartRunT4MVC", false));

                    Settings.GoogleClosureJavaPath = Convert.ToString(regKey.GetValue("GoogleClosureJavaPath", string.Empty));
                    Settings.GoogleClosureOffline = Convert.ToBoolean(regKey.GetValue("GoogleClosureOffline", false));

                    Settings.RunJSHint = Convert.ToBoolean(regKey.GetValue("RunJSHint", true));
                    Settings.ShowDetailLog = Convert.ToBoolean(regKey.GetValue("ShowDetailLog", true));
                }

                LoadExtensions();
            } 
            catch (Exception ex)
            {
                Debug.WriteLine("Chrip - failed to load: " + ex.Message);
                System.Windows.Forms.MessageBox.Show("Chrip - failed to load: " + ex.Message);
            }
            finally 
            {
                if (regKey != null)
                {
                    regKey.Close();
                }
            }
        }

        /// <summary>
        /// Saves options page settings to registry.
        /// </summary>
        public static void Save() 
        {
            using (var regKey = Registry.CurrentUser.OpenSubKey(RegWDS, true) ?? Registry.CurrentUser.CreateSubKey(RegWDS))
            {
                regKey.SetValue("ChirpCssFile", Settings.ChirpCssFile);
                regKey.SetValue("ChirpHybridCssFile", Settings.ChirpHybridCssFile);
                regKey.SetValue("ChirpMichaelAshCssFile", Settings.ChirpMichaelAshCssFile);
                regKey.SetValue("ChirpMSAjaxCssFile", Settings.ChirpMSAjaxCssFile);
                regKey.SetValue("ChirpJsFile", Settings.ChirpJsFile);
                regKey.SetValue("ChirpLessFile", Settings.ChirpLessFile);
                regKey.SetValue("ChirpHybridLessFile", Settings.ChirpHybridLessFile);
                regKey.SetValue("ChirpMichaelAshLessFile", Settings.ChirpMichaelAshLessFile);
                regKey.SetValue("ChirpMSAjaxLessFile", Settings.ChirpMSAjaxLessFile);
                regKey.SetValue("ChirpSimpleJsFile", Settings.ChirpSimpleJsFile);
                regKey.SetValue("ChirpWhiteSpaceJsFile", Settings.ChirpWhiteSpaceJsFile);
                regKey.SetValue("ChirpYUIJsFile", Settings.ChirpYUIJsFile);
                regKey.SetValue("ChirpGcJsFile", Settings.ChirpGctJsFile);
                regKey.SetValue("ChirpMSAjaxJsFile", Settings.ChirpMSAjaxJsFile);
                regKey.SetValue("ChirpConfigFile", Settings.ChirpConfigFile);
                regKey.SetValue("DefaultCssMinifier", Settings.DefaultCssMinifier.ToString());
                regKey.SetValue("DefaultJavaScriptMinifier", Settings.DefaultJavaScriptMinifier.ToString());

                regKey.SetValue("ChirpUglifyJsFile", Settings.ChirpUglifyJsFile);

                regKey.SetValue("ChirpSimpleCoffeeScriptFile", Settings.ChirpSimpleCoffeeScriptFile);
                regKey.SetValue("ChirpWhiteSpaceCoffeeScriptFile", Settings.ChirpWhiteSpaceCoffeeScriptFile);
                regKey.SetValue("ChirpYUICoffeeScriptFile", Settings.ChirpYUICoffeeScriptFile);
                regKey.SetValue("ChirpGcCoffeeScriptFile", Settings.ChirpGctCoffeeScriptFile);
                regKey.SetValue("ChirpMSAjaxCoffeeScriptFile", Settings.ChirpMSAjaxCoffeeScriptFile);
                regKey.SetValue("ChirpUglifyCoffeeScriptFile", Settings.ChirpUglifyCoffeeScriptFile);
                regKey.SetValue("CoffeeScriptBatFilePath", Settings.CoffeeScriptBatFilePath);

                regKey.SetValue("T4RunAsBuild", Settings.T4RunAsBuild.ToString());
                regKey.SetValue("T4RunAsBuildTemplate", Settings.T4RunAsBuildTemplate.ToString());

                regKey.SetValue("SmartRunT4MVC", Settings.SmartRunT4MVC.ToString());

                regKey.SetValue("GoogleClosureJavaPath", Settings.GoogleClosureJavaPath);
                regKey.SetValue("GoogleClosureOffline", Settings.GoogleClosureOffline);

                regKey.SetValue("RunJSHint", Settings.RunJSHint);
                regKey.SetValue("showDetailLog", Settings.showDetailLog);

                LoadExtensions();

                if (Saved != null)
                { 
                    Saved(); 
                }
            }
        }
        #endregion

        private static void LoadExtensions()
        {
            AllExtensions = new[]
            {
                 Settings.ChirpConfigFile, Settings.ChirpCssFile, Settings.ChirpGctJsFile, Settings.ChirpHybridCssFile, Settings.ChirpHybridLessFile, Settings.ChirpJsFile, Settings.ChirpLessFile, Settings.ChirpMichaelAshCssFile, Settings.ChirpMichaelAshLessFile,
                 Settings.ChirpMSAjaxCssFile, Settings.ChirpMSAjaxJsFile, Settings.ChirpMSAjaxLessFile, Settings.ChirpPartialViewFile, Settings.ChirpSimpleJsFile, Settings.ChirpViewFile, Settings.ChirpWhiteSpaceJsFile, Settings.ChirpYUIJsFile,
                 Settings.ChirpSimpleCoffeeScriptFile, Settings.ChirpWhiteSpaceCoffeeScriptFile, Settings.ChirpYUICoffeeScriptFile, Settings.ChirpMSAjaxCoffeeScriptFile, Settings.ChirpGctCoffeeScriptFile, Settings.ChirpCoffeeScriptFile,
                ".debug.js", ".debug.css"
            };
        }
    }
}
