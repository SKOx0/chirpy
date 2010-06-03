using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Zippy.Chirp
{
    public partial class ConfigurationScreen : UserControl, EnvDTE.IDTToolsOptionsPage
    {
        public ConfigurationScreen()
        {
            InitializeComponent();
        }


        #region IDTToolsOptionsPage Members
        void EnvDTE.IDTToolsOptionsPage.GetProperties(ref object PropertiesObject)
        {
            PropertiesObject = null;
        }

        void EnvDTE.IDTToolsOptionsPage.OnAfterCreated(EnvDTE.DTE DTEObject)
        {
            Settings.Load();
            txtChirpConfigFile.Text = Settings.ChirpConfigFile;
            txtChirpCssFile.Text = Settings.ChirpCssFile;
            txtChirpJsFile.Text = Settings.ChirpJsFile;
            txtChirpLessFile.Text = Settings.ChirpLessFile;
            txtChirpSimpleJsFile.Text = Settings.ChirpSimpleJsFile;
            txtChirpWhiteSpaceJsFile.Text = Settings.ChirpWhiteSpaceJsFile;
            txtChirpYUIJsFile.Text = Settings.ChirpYUIJsFile;
        }

        void EnvDTE.IDTToolsOptionsPage.OnCancel()
        {
            
        }

        void EnvDTE.IDTToolsOptionsPage.OnHelp()
        {
            
        }

        void EnvDTE.IDTToolsOptionsPage.OnOK()
        {
            Settings.ChirpConfigFile=txtChirpConfigFile.Text;
            Settings.ChirpCssFile=txtChirpCssFile.Text;
            Settings.ChirpJsFile=txtChirpJsFile.Text;
            Settings.ChirpLessFile=txtChirpLessFile.Text;
            Settings.ChirpSimpleJsFile=txtChirpSimpleJsFile.Text;
            Settings.ChirpWhiteSpaceJsFile=txtChirpWhiteSpaceJsFile.Text;
            Settings.ChirpYUIJsFile=txtChirpYUIJsFile.Text;
            Settings.Save();
        }
        #endregion
    }
}
