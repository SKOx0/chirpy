using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Zippy.Chirp.ConfigurationScreen
{
    public partial class GoogleClosure : UserControl, EnvDTE.IDTToolsOptionsPage
    {
        public GoogleClosure()
        {
            InitializeComponent();
          }

        void EnvDTE.IDTToolsOptionsPage.GetProperties(ref object propertiesObject)
        {
            propertiesObject = null;
        }

        void EnvDTE.IDTToolsOptionsPage.OnAfterCreated(EnvDTE.DTE dteObject)
        {
            Settings.Load();

            this.textBoxJavaPath.Text = Settings.GoogleClosureJavaPath;
            this.toolTip1.SetToolTip(textBoxJavaPath, textBoxJavaPath.Text);
            this.chkEnableOfline.Checked = Settings.GoogleClosureOffline;
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
            Settings.GoogleClosureOffline = this.chkEnableOfline.Checked;
            Settings.GoogleClosureJavaPath = this.textBoxJavaPath.Text;
            Settings.Save();
        }

        private void EnableOfline_CheckedChanged(object sender, EventArgs e)
        {
            this.groupBoxOffline.Enabled = this.chkEnableOfline.Checked;
            if (string.IsNullOrEmpty(textBoxJavaPath.Text))
            {
                this.FindJavaPath();
            }
        }

        private void FindJava_Click(object sender, EventArgs e)
        {
            if (!this.FindJavaPath())
            {
                // active textbox if java path is not present in registry
                this.textBoxJavaPath.Enabled = true;

                // not found in registry, ask user to find java.exe
                if (MessageBox.Show(
                    this,
                    string.Format("Java runtime was not found in the registry.{0}Do you wish to try to find javaw.exe?", Environment.NewLine),
                    "Chirpy needs Javaw.exe..",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question)
                    == DialogResult.Yes)
                {
                    openFileDialog1.Filter = "javaw.exe";
                    openFileDialog1.ShowDialog(this);
                    this.GotJavaPath(openFileDialog1.FileName);
                }
            }
        }

        private bool FindJavaPath()
        {
            // Try registry first
            var regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\JavaSoft\Java Runtime Environment")
                ??
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\JavaSoft\Java Runtime Environment");

            if (regKey != null)
            {
                var currentVersion = Convert.ToString(regKey.GetValue("CurrentVersion", string.Empty));
                if (!string.IsNullOrEmpty(currentVersion))
                {
                    if (this.GotJavaPath(Convert.ToString(regKey.OpenSubKey(currentVersion).GetValue("JavaHome", string.Empty)) + @"\bin\javaw.exe"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool GotJavaPath(string path)
        {
            if (System.IO.File.Exists(path))
            {
                this.textBoxJavaPath.Text = path;
                return true;
            }

            return false;
        }
    }
}
