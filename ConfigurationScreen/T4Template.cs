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

        void EnvDTE.IDTToolsOptionsPage.GetProperties(ref object propertiesObject)
        {
            propertiesObject = null;
        }

        void EnvDTE.IDTToolsOptionsPage.OnAfterCreated(EnvDTE.DTE DTEObject)
        {
            Settings.Load();
            this.chkT4RunOnBuild.Checked = Settings.T4RunAsBuild;
            this.txtT4RunAsBuildTemplate.Enabled = chkT4RunOnBuild.Checked;
            this.txtT4RunAsBuildTemplate.Text = Settings.T4RunAsBuildTemplate;
            this.chkSmartRunT4MVC.Checked = Settings.SmartRunT4MVC;
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
            Settings.T4RunAsBuild = this.chkT4RunOnBuild.Checked;
            Settings.T4RunAsBuildTemplate = this.txtT4RunAsBuildTemplate.Text;
            Settings.SmartRunT4MVC = this.chkSmartRunT4MVC.Checked;
            Settings.Save();
        }

        private void T4RunOnBuild_CheckedChanged(object sender, EventArgs e)
        {
            this.txtT4RunAsBuildTemplate.Enabled = this.chkT4RunOnBuild.Checked;
        }
    }
}
