﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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
        private const string RegWDSJsHint = @"SOFTWARE\Microsoft\VisualStudio\10.0\Chirp\JSHint";
        private const string RegWDSCssLint = @"SOFTWARE\Microsoft\VisualStudio\10.0\Chirp\CSSLint";
        private string chirpJsFile = ".chirp.js";
        private string chirpSimpleJsFile = ".simple.js";
        private string chirpWhiteSpaceJsFile = ".whitespace.js";
        private string chirpYUIJsFile = ".yui.js";
        private string chirpGctJsFile = ".gct.js";
        private string chirpMSAjaxJsFile = ".msajax.js";
        private string outputExtensionJS = ".min.js";
        private string outputExtensionCSS = ".min.css";
        private string chirpPartialViewFile = ".chirp.ascx";
        private string chirpMichaelAshLessFile = ".michaelash.less";
        private string chirpViewFile = ".chirp.aspx";
        private string chirpRazorCSViewFile = ".chirp.cshtml";
        private string chirpRazorVBViewFile = ".chirp.vbhtml";
        private string chirpMSAjaxLessFile = ".msajax.less";
        private string chirpLessFile = ".chirp.less";
        private string chirpHybridLessFile = ".hybrid.less";
        private string chirpCoffeeScriptFile = ".chirp.coffee";
        private string chirpUglifyJsFile = ".uglify.js";
        private string chirpSimpleCoffeeScriptFile = ".simple.coffee";
        private string chirpWhiteSpaceCoffeeScriptFile = ".whitespace.coffee";
        private string chirpYUICoffeeScriptFile = ".yui.coffee";
        private string chirpGctCoffeeScriptFile = ".gct.coffee";
        private string coffeeScriptBatFilePath = string.Empty;
        private string chirpCssFile = ".chirp.css";
        private string chirpMSAjaxCssFile = ".msajax.css";
        private string chirpHybridCssFile = ".hybrid.css";
        private string chirpMichaelAshCssFile = ".michaelash.css";
        private bool t4RunAsBuild = false;
        private string t4RunAsBuildTemplate = string.Empty;
        private bool smartRunT4MVC = false;
        private bool runJSHint = true;
        private bool runCSSLint = true;
        private bool googleClosureOffline = false;
        private string googleClosureJavaPath = string.Empty;
        private string chirpMSAjaxCoffeeScriptFile = ".msajax.coffee";
        private string chirpUglifyCoffeeScriptFile = ".uglify.coffee";
        private string chirpConfigFile = ".chirp.config";
        private bool showDetailLog = true;
        private string[] allExtensions;
        private Xml.MinifyType defaultCssMinifier = Xml.MinifyType.yui;
        private Xml.MinifyType defaultJavaScriptMinifier = Xml.MinifyType.yui;
        #endregion

        #region Constructors
        public Settings()
        {
            this.LoadSettingFromRegistry();
        }

        public Settings(string directoryPath)
        {
            // load defaut setting from registry
            this.LoadSettingFromRegistry();

            if (!string.IsNullOrEmpty(directoryPath))
            {
                this.SetSettings(this.CalculateSettingsForDirectory(directoryPath, new Dictionary<string, string>(), this.ChirpConfigFile));
            }
        }
        #endregion

        public static event Action Saved;

        #region Properties

        public string ChirpJsFile
        {
            get { return this.chirpJsFile; }
            set { this.chirpJsFile = value; }
        }

        public string ChirpSimpleJsFile
        {
            get { return this.chirpSimpleJsFile; }
            set { this.chirpSimpleJsFile = value; }
        }

        public string ChirpWhiteSpaceJsFile
        {
            get { return this.chirpWhiteSpaceJsFile; }
            set { this.chirpWhiteSpaceJsFile = value; }
        }

        public string ChirpYUIJsFile
        {
            get { return this.chirpYUIJsFile; }
            set { this.chirpYUIJsFile = value; }
        }

        public string ChirpGctJsFile
        {
            get { return this.chirpGctJsFile; }
            set { this.chirpGctJsFile = value; }
        }

        public string ChirpMSAjaxJsFile
        {
            get { return this.chirpMSAjaxJsFile; }
            set { this.chirpMSAjaxJsFile = value; }
        }

        public string ChirpPartialViewFile
        {
            get { return this.chirpPartialViewFile; }
            set { this.chirpPartialViewFile = value; }
        }

        public string ChirpViewFile
        {
            get { return this.chirpViewFile; }
            set { this.chirpViewFile = value; }
        }

        public string ChirpRazorCSViewFile
        {
            get { return this.chirpRazorCSViewFile; }
            set { this.chirpRazorCSViewFile = value; }
        }

        public string ChirpRazorVBViewFile
        {
            get { return this.chirpRazorVBViewFile; }
            set { this.chirpRazorVBViewFile = value; }
        }

        public string ChirpLessFile
        {
            get { return this.chirpLessFile; }
            set { this.chirpLessFile = value; }
        }

        public string ChirpMSAjaxLessFile
        {
            get { return this.chirpMSAjaxLessFile; }
            set { this.chirpMSAjaxLessFile = value; }
        }

        public string ChirpHybridLessFile
        {
            get { return this.chirpHybridLessFile; }
            set { this.chirpHybridLessFile = value; }
        }

        public string ChirpMichaelAshLessFile
        {
            get { return this.chirpMichaelAshLessFile; }
            set { this.chirpMichaelAshLessFile = value; }
        }

        public string ChirpUglifyJsFile
        {
            get { return this.chirpUglifyJsFile; }
            set { this.chirpUglifyJsFile = value; }
        }

        public string ChirpCoffeeScriptFile
        {
            get { return this.chirpCoffeeScriptFile; }
            set { this.chirpCoffeeScriptFile = value; }
        }

        public string ChirpSimpleCoffeeScriptFile
        {
            get { return this.chirpSimpleCoffeeScriptFile; }
            set { this.chirpSimpleCoffeeScriptFile = value; }
        }

        public string ChirpWhiteSpaceCoffeeScriptFile
        {
            get { return this.chirpWhiteSpaceCoffeeScriptFile; }
            set { this.chirpWhiteSpaceCoffeeScriptFile = value; }
        }

        public string ChirpYUICoffeeScriptFile
        {
            get { return this.chirpYUICoffeeScriptFile; }
            set { this.chirpYUICoffeeScriptFile = value; }
        }

        public string ChirpGctCoffeeScriptFile
        {
            get { return this.chirpGctCoffeeScriptFile; }
            set { this.chirpGctCoffeeScriptFile = value; }
        }

        public string ChirpMSAjaxCoffeeScriptFile
        {
            get { return this.chirpMSAjaxCoffeeScriptFile; }
            set { this.chirpMSAjaxCoffeeScriptFile = value; }
        }

        public string ChirpUglifyCoffeeScriptFile
        {
            get { return this.chirpUglifyCoffeeScriptFile; }
            set { this.chirpUglifyCoffeeScriptFile = value; }
        }

        public string CoffeeScriptBatFilePath
        {
            get { return this.coffeeScriptBatFilePath; }
            set { this.coffeeScriptBatFilePath = value; }
        }

        public string ChirpCssFile
        {
            get { return this.chirpCssFile; }
            set { this.chirpCssFile = value; }
        }

        public string ChirpMSAjaxCssFile
        {
            get { return this.chirpMSAjaxCssFile; }
            set { this.chirpMSAjaxCssFile = value; }
        }

        public string ChirpHybridCssFile
        {
            get { return this.chirpHybridCssFile; }
            set { this.chirpHybridCssFile = value; }
        }

        public string ChirpMichaelAshCssFile
        {
            get { return this.chirpMichaelAshCssFile; }
            set { this.chirpMichaelAshCssFile = value; }
        }

        public string ChirpConfigFile
        {
            get { return this.chirpConfigFile; }
            set { this.chirpConfigFile = value; }
        }

        public Xml.MinifyType DefaultCssMinifier
        {
            get { return this.defaultCssMinifier; }
            set { this.defaultCssMinifier = value; }
        }

        public Xml.MinifyType DefaultJavaScriptMinifier
        {
            get { return this.defaultJavaScriptMinifier; }
            set { this.defaultJavaScriptMinifier = value; }
        }

        public string[] AllExtensions
        {
            get { return this.allExtensions; }
            set { this.allExtensions = value; }
        }

        public JavaScript.JSHint.options JsHintOptions
        {
            get;
            set;
        }

        public JavaScript.CSSLint.options CssLintOptions
        {
            get;
            set;
        }

        public bool T4RunAsBuild
        {
            get { return this.t4RunAsBuild; }
            set { this.t4RunAsBuild = value; }
        }

        public string T4RunAsBuildTemplate
        {
            get { return this.t4RunAsBuildTemplate; }
            set { this.t4RunAsBuildTemplate = value; }
        }

        public bool SmartRunT4MVC
        {
            get { return this.smartRunT4MVC; }
            set { this.smartRunT4MVC = value; }
        }

        public bool RunJSHint
        {
            get { return this.runJSHint; }
            set { this.runJSHint = value; }
        }

        public bool RunCSSLint
        {
            get { return this.runCSSLint; }
            set { this.runCSSLint = value; }
        }

        public string OutputExtensionJS
        {
            get { return this.outputExtensionJS; }
            set { this.outputExtensionJS = value; }
        }

        public string OutputExtensionCSS
        {
            get { return this.outputExtensionCSS; }
            set { this.outputExtensionCSS = value; }
        }

        public bool GoogleClosureOffline
        {
            get { return this.googleClosureOffline; }
            set { this.googleClosureOffline = value; }
        }

        public string GoogleClosureJavaPath
        {
            get { return this.googleClosureJavaPath; }
            set { this.googleClosureJavaPath = value; }
        }

        public bool ShowDetailLog
        {
            get { return this.showDetailLog; }
            set { this.showDetailLog = value; }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Instance settings class
        /// </summary>
        /// <param name="fullFileName">Full file name</param>
        /// <returns>Instance settings class by full file name</returns>
        public static Settings Instance(string fullFileName)
        {
            return new Settings(System.IO.Path.GetDirectoryName(fullFileName));
        }

        /// <summary>
        /// Saves options page settings to registry.
        /// </summary>
        public void Save()
        {
            using (var regKey = Registry.CurrentUser.OpenSubKey(RegWDS, true) ?? Registry.CurrentUser.CreateSubKey(RegWDS))
            {
                regKey.SetValue("ChirpCssFile", this.ChirpCssFile);
                regKey.SetValue("ChirpHybridCssFile", this.ChirpHybridCssFile);
                regKey.SetValue("ChirpMichaelAshCssFile", this.ChirpMichaelAshCssFile);
                regKey.SetValue("ChirpMSAjaxCssFile", this.ChirpMSAjaxCssFile);
                regKey.SetValue("ChirpJsFile", this.ChirpJsFile);
                regKey.SetValue("ChirpLessFile", this.ChirpLessFile);
                regKey.SetValue("ChirpHybridLessFile", this.ChirpHybridLessFile);
                regKey.SetValue("ChirpMichaelAshLessFile", this.ChirpMichaelAshLessFile);
                regKey.SetValue("ChirpMSAjaxLessFile", this.ChirpMSAjaxLessFile);
                regKey.SetValue("ChirpSimpleJsFile", this.ChirpSimpleJsFile);
                regKey.SetValue("ChirpWhiteSpaceJsFile", this.ChirpWhiteSpaceJsFile);
                regKey.SetValue("ChirpYUIJsFile", this.ChirpYUIJsFile);
                regKey.SetValue("ChirpGcJsFile", this.ChirpGctJsFile);
                regKey.SetValue("ChirpMSAjaxJsFile", this.ChirpMSAjaxJsFile);
                regKey.SetValue("ChirpConfigFile", this.ChirpConfigFile);
                regKey.SetValue("DefaultCssMinifier", this.DefaultCssMinifier.ToString());
                regKey.SetValue("DefaultJavaScriptMinifier", this.DefaultJavaScriptMinifier.ToString());

                regKey.SetValue("ChirpUglifyJsFile", this.ChirpUglifyJsFile);

                regKey.SetValue("ChirpSimpleCoffeeScriptFile", this.ChirpSimpleCoffeeScriptFile);
                regKey.SetValue("ChirpWhiteSpaceCoffeeScriptFile", this.ChirpWhiteSpaceCoffeeScriptFile);
                regKey.SetValue("ChirpYUICoffeeScriptFile", this.ChirpYUICoffeeScriptFile);
                regKey.SetValue("ChirpGcCoffeeScriptFile", this.ChirpGctCoffeeScriptFile);
                regKey.SetValue("ChirpMSAjaxCoffeeScriptFile", this.ChirpMSAjaxCoffeeScriptFile);
                regKey.SetValue("ChirpUglifyCoffeeScriptFile", this.ChirpUglifyCoffeeScriptFile);
                regKey.SetValue("CoffeeScriptBatFilePath", this.CoffeeScriptBatFilePath);

                regKey.SetValue("OutputExtensionCSS", this.outputExtensionCSS);
                regKey.SetValue("OutputExtensionJS", this.outputExtensionJS);

                regKey.SetValue("T4RunAsBuild", this.T4RunAsBuild.ToString());
                regKey.SetValue("T4RunAsBuildTemplate", this.T4RunAsBuildTemplate.ToString());

                regKey.SetValue("SmartRunT4MVC", this.SmartRunT4MVC.ToString());

                regKey.SetValue("GoogleClosureJavaPath", this.GoogleClosureJavaPath);
                regKey.SetValue("GoogleClosureOffline", this.GoogleClosureOffline);

                regKey.SetValue("RunJSHint", this.RunJSHint);
                regKey.SetValue("RunCSSLint", this.RunCSSLint);
                regKey.SetValue("showDetailLog", this.showDetailLog);

                this.SaveOptionsInRegistry(RegWDSJsHint, this.JsHintOptions);
                this.SaveOptionsInRegistry(RegWDSCssLint, this.CssLintOptions);

                this.LoadExtensions();

                if (Saved != null)
                {
                    Saved();
                }
            }
        }
        #endregion

        /// <summary>
        /// Loads options page settings from registry.
        /// </summary>
        private void LoadSettingFromRegistry()
        {
            RegistryKey regKey = null;
            try
            {
                regKey = Registry.CurrentUser.OpenSubKey(RegWDS, false);
                if (regKey != null)
                {
                    this.ChirpJsFile = Convert.ToString(regKey.GetValue("ChirpJsFile", ".chirp.js"));
                    this.ChirpSimpleJsFile = Convert.ToString(regKey.GetValue("ChirpSimpleJsFile", ".simple.js"));
                    this.ChirpWhiteSpaceJsFile = Convert.ToString(regKey.GetValue("ChirpWhiteSpaceJsFile", ".whitespace.js"));
                    this.ChirpYUIJsFile = Convert.ToString(regKey.GetValue("ChirpYUIJsFile", ".yui.js"));
                    this.ChirpGctJsFile = Convert.ToString(regKey.GetValue("ChirpGcJsFile", ".gct.js"));
                    this.ChirpMSAjaxJsFile = Convert.ToString(regKey.GetValue("ChirpMSAjaxJsFile", ".msajax.js"));
                    this.ChirpLessFile = Convert.ToString(regKey.GetValue("ChirpLessFile", ".chirp.less"));
                    this.ChirpHybridLessFile = Convert.ToString(regKey.GetValue("ChirpHybridLessFile", ".hybrid.less"));
                    this.ChirpMichaelAshLessFile = Convert.ToString(regKey.GetValue("ChirpMichaelAshLessFile", ".michaelash.less"));
                    this.ChirpMSAjaxLessFile = Convert.ToString(regKey.GetValue("ChirpMSAjaxLessFile", ".msajax.less"));
                    this.ChirpUglifyJsFile = Convert.ToString(regKey.GetValue("ChirpUglifyJsFile", ".uglify.js"));

                    this.ChirpCoffeeScriptFile = Convert.ToString(regKey.GetValue("ChirpCoffeeScriptFile", ".chirp.coffee"));
                    this.ChirpSimpleCoffeeScriptFile = Convert.ToString(regKey.GetValue("ChirpSimpleCoffeeScriptFile", ".simple.coffee"));
                    this.ChirpWhiteSpaceCoffeeScriptFile = Convert.ToString(regKey.GetValue("ChirpWhiteSpaceCoffeeScriptFile", ".whitespace.coffee"));
                    this.ChirpYUICoffeeScriptFile = Convert.ToString(regKey.GetValue("ChirpYUICoffeeScriptFile", ".yui.coffee"));
                    this.ChirpGctCoffeeScriptFile = Convert.ToString(regKey.GetValue("ChirpGcCoffeeScriptFile", ".gct.coffee"));
                    this.ChirpMSAjaxCoffeeScriptFile = Convert.ToString(regKey.GetValue("ChirpMSAjaxCoffeeScriptFile", ".msajax.coffee"));
                    this.ChirpUglifyCoffeeScriptFile = Convert.ToString(regKey.GetValue("ChirpUglifyCoffeeScriptFile", ".uglify.coffee"));
                    this.CoffeeScriptBatFilePath = Convert.ToString(regKey.GetValue("CoffeeScriptBatFilePath", string.Empty));

                    // Settings.ChirpLessCssFile = Convert.ToString(regKey.GetValue("ChirpLessCssFile", ".chirp.less.css"));
                    this.ChirpCssFile = Convert.ToString(regKey.GetValue("ChirpCssFile", ".chirp.css"));
                    this.ChirpHybridCssFile = Convert.ToString(regKey.GetValue("ChirpHybridCssFile", ".hybrid.css"));
                    this.ChirpMichaelAshCssFile = Convert.ToString(regKey.GetValue("ChirpMichaelAshCssFile", ".michaelash.css"));
                    this.ChirpMSAjaxCssFile = Convert.ToString(regKey.GetValue("ChirpMSAjaxCssFile", ".msajax.css"));
                    this.ChirpConfigFile = Convert.ToString(regKey.GetValue("ChirpConfigFile", ".chirp.config"));
                    this.DefaultCssMinifier = Convert.ToString(regKey.GetValue("DefaultCssMinifier", string.Empty)).ToEnum(Xml.MinifyType.yui);
                    this.DefaultJavaScriptMinifier = Convert.ToString(regKey.GetValue("DefaultJavaScriptMinifier", string.Empty)).ToEnum(Xml.MinifyType.yui);

                    this.OutputExtensionCSS = Convert.ToString(regKey.GetValue("OutputExtensionCSS", ".min.css"));
                    this.OutputExtensionJS = Convert.ToString(regKey.GetValue("OutputExtensionJS", ".min.js"));

                    this.T4RunAsBuild = Convert.ToBoolean(regKey.GetValue("T4RunAsBuild", false));
                    this.T4RunAsBuildTemplate = Convert.ToString(regKey.GetValue("T4RunAsBuildTemplate", "T4MVC.tt,NHibernateMapping.tt"));
                    this.SmartRunT4MVC = Convert.ToBoolean(regKey.GetValue("SmartRunT4MVC", false));

                    this.GoogleClosureJavaPath = Convert.ToString(regKey.GetValue("GoogleClosureJavaPath", string.Empty));
                    this.GoogleClosureOffline = Convert.ToBoolean(regKey.GetValue("GoogleClosureOffline", false));

                    this.RunJSHint = Convert.ToBoolean(regKey.GetValue("RunJSHint", true));
                    this.RunCSSLint = Convert.ToBoolean(regKey.GetValue("RunCSSLint", true));
                    this.ShowDetailLog = Convert.ToBoolean(regKey.GetValue("ShowDetailLog", true));
                }

                this.LoadJsHintOptions();
                this.LoadCssLintOptions();

                this.LoadExtensions();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Chirpy - failed to load registry: " + ex.Message);
                System.Windows.Forms.MessageBox.Show("Chirpy - failed to load registry: " + ex.Message);
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
        /// set setting from list
        /// </summary>
        /// <param name="settings">list of settings key-value</param>
        private void SetSettings(IEnumerable<KeyValuePair<string, string>> settings)
        {
            foreach (var kvp in settings)
            {
                var thisType = typeof(Settings);
                System.Reflection.PropertyInfo prop = thisType.GetProperty(kvp.Key);
                if (prop == null)
                {
                    continue;
                }

                if (prop.PropertyType == typeof(string))
                {
                    prop.SetValue(this, kvp.Value, null);
                }
                else if (prop.PropertyType == typeof(bool))
                {
                    prop.SetValue(this, Convert.ToBoolean(kvp.Value), null);
                }
                else if (prop.PropertyType == typeof(int))
                {
                    prop.SetValue(this, Convert.ToInt16(kvp.Value), null);
                }
            }
        }

        private IDictionary<string, string> CalculateSettingsForDirectory(string directoryPath, IDictionary<string, string> initialSettings, string configFile)
        {
            foreach (var file in Directory.GetFiles(directoryPath, "*" + configFile, SearchOption.TopDirectoryOnly))
            {
                // Process the file
                // /root/Settings/key
                try
                {
                    // var xdoc = new System.Xml.XPath.XPathDocument(file).CreateNavigator();
                    var xdoc = System.Xml.Linq.XDocument.Load(file);
                    System.Xml.Linq.XNamespace c = "urn:ChirpyConfig";
                    var settings = xdoc.Element(c + "root").Element(c + "Settings");
                    if (settings != null)
                    {
                        foreach (var setting in settings.Elements(c + "Setting"))
                        {
                            if (!initialSettings.ContainsKey(setting.Attribute("key").Value))
                            {
                                initialSettings.Add(setting.Attribute("key").Value,
                                                    setting.Attribute("value").Value);
                            }
                        }
                    }
                }
                catch (System.IO.IOException)
                {
                    // Ignore locked files, etc.
                }
            }

            var parent = System.IO.Directory.GetParent(directoryPath);
            if (parent != null)
            {
                this.CalculateSettingsForDirectory(parent.FullName, initialSettings, configFile);
            }

            return initialSettings;
        }

        private void LoadJsHintOptions()
        {
            if (this.JsHintOptions == null)
            {
                this.JsHintOptions = new JavaScript.JSHint.options();

                // default setting
                this.JsHintOptions.devel = true;
                this.JsHintOptions.curly = true;
                this.JsHintOptions.undef = true;

                this.LoadOptionsFromRegistry(RegWDSJsHint, this.JsHintOptions);
            }
        }

        private void LoadCssLintOptions()
        {
            if (this.CssLintOptions == null)
            {
                this.CssLintOptions = new JavaScript.CSSLint.options();
                this.LoadOptionsFromRegistry(RegWDSCssLint, this.CssLintOptions);
            }
        }

        private void LoadOptionsFromRegistry<T>(string regKey, T objectToSave) {
            LoadOptionsFromRegistry(regKey, typeof(T), objectToSave);
        }

        private void LoadOptionsFromRegistry(string regKey, Type objectType, object objectToSave)
        {
            RegistryKey regKeyOptions = null;
            try
            {
                regKeyOptions = Registry.CurrentUser.OpenSubKey(regKey, false);
                if (regKeyOptions != null)
                {
                    PropertyInfo[] propertyInfos;
                    propertyInfos = objectType.GetProperties();
                    foreach (PropertyInfo propertyInfo in propertyInfos)
                    {
                        try
                        {
                            if (propertyInfo.PropertyType == typeof(bool))
                            {
                                bool tempValue = false;
                                if (bool.TryParse(regKeyOptions.GetValue(propertyInfo.Name, false).ToString(), out tempValue))
                                {
                                    propertyInfo.SetValue(objectToSave, tempValue, null);
                                }
                            }
                            else if (propertyInfo.PropertyType == typeof(string))
                            {
                                propertyInfo.SetValue(objectToSave, regKeyOptions.GetValue(propertyInfo.Name), null);
                            }
                            else if (propertyInfo.PropertyType == typeof(int))
                            {
                                propertyInfo.SetValue(objectToSave, Convert.ToInt16(regKeyOptions.GetValue(propertyInfo.Name)), null);
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Windows.Forms.MessageBox.Show("Chrip - failed to load: " + propertyInfo.Name + "=" + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Chrip - failed to load: " + ex.Message);
                System.Windows.Forms.MessageBox.Show("Chrip - failed to load: " + ex.Message);
            }
            finally
            {
                if (regKeyOptions != null)
                {
                    regKeyOptions.Close();
                }
            }
        }

        private void SaveOptionsInRegistry<T>(string regKey,T objectToSave) {
            SaveOptionsInRegistry(regKey, typeof(T), objectToSave);
        }

        private void SaveOptionsInRegistry(string regKey, Type objectType, object objectToSave)
        {
            using (var regKeyOptions = Registry.CurrentUser.OpenSubKey(regKey, true) ?? Registry.CurrentUser.CreateSubKey(regKey))
            {
                PropertyInfo[] propertyInfos;
                propertyInfos = objectType.GetProperties();
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    regKeyOptions.SetValue(propertyInfo.Name, propertyInfo.GetValue(objectToSave, null));
                }
            }
        }

        private void LoadExtensions()
        {
            this.AllExtensions = new[]
            {
                 this.ChirpConfigFile, this.ChirpCssFile, this.ChirpGctJsFile, this.ChirpHybridCssFile, this.ChirpHybridLessFile, this.ChirpJsFile, this.ChirpLessFile, this.ChirpMichaelAshCssFile, this.ChirpMichaelAshLessFile,
                 this.ChirpMSAjaxCssFile, this.ChirpMSAjaxJsFile, this.ChirpMSAjaxLessFile, this.ChirpPartialViewFile, this.ChirpSimpleJsFile, this.ChirpViewFile, this.ChirpWhiteSpaceJsFile, this.ChirpYUIJsFile,
                 this.ChirpSimpleCoffeeScriptFile, this.ChirpWhiteSpaceCoffeeScriptFile, this.ChirpYUICoffeeScriptFile, this.ChirpMSAjaxCoffeeScriptFile, this.ChirpGctCoffeeScriptFile, this.ChirpCoffeeScriptFile,
                ".debug.js", ".debug.css"
            };
        }
    }
}
