using System;
using System.Windows.Forms;

namespace Zippy.Chirp.ConfigurationScreen
{
    public partial class T4Template : UserControl, EnvDTE.IDTToolsOptionsPage
    {
        public T4Template()
        {
            InitializeComponent();
        }

        void EnvDTE.IDTToolsOptionsPage.GetProperties(ref object PropertiesObject)
        {
            PropertiesObject = null;
        }

        void EnvDTE.IDTToolsOptionsPage.OnAfterCreated(EnvDTE.DTE DTEObject)
        {
            Settings.Load();
             chkT4RunOnBuild.Checked = Settings.T4RunAsBuild;
            txtT4RunAsBuildTemplate.Enabled = chkT4RunOnBuild.Checked;
            txtT4RunAsBuildTemplate.Text = Settings.T4RunAsBuildTemplate;
            chkSmartRunT4MVC.Checked = Settings.SmartRunT4MVC;
 
        }

        void EnvDTE.IDTToolsOptionsPage.OnCancel()
        {
            throw new NotImplementedException();
        }

        void EnvDTE.IDTToolsOptionsPage.OnHelp()
        {
            System.Diagnostics.Process.Start("http://chirpy.codeplex.com/");
        }

        void EnvDTE.IDTToolsOptionsPage.OnOK()
        {
             Settings.T4RunAsBuild = chkT4RunOnBuild.Checked;
            Settings.T4RunAsBuildTemplate = txtT4RunAsBuildTemplate.Text;
            Settings.SmartRunT4MVC = chkSmartRunT4MVC.Checked;
            Settings.Save();
        }

        private void chkT4RunOnBuild_CheckedChanged(object sender, EventArgs e)
        {
            txtT4RunAsBuildTemplate.Enabled = chkT4RunOnBuild.Checked;
        }
    }
}
