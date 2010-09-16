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

        void EnvDTE.IDTToolsOptionsPage.GetProperties(ref object PropertiesObject)
        {
            PropertiesObject = null;
        }

        void EnvDTE.IDTToolsOptionsPage.OnAfterCreated(EnvDTE.DTE DTEObject)
        {
            Settings.Load();

            textBoxJavaPath.Text = Settings.GoogleClosureJavaPath;
            toolTip1.SetToolTip(textBoxJavaPath, textBoxJavaPath.Text);
            chkEnableOfline.Checked=Settings.GoogleClosureOffline;
            //chkEnableOfline.Checked = !string.IsNullOrEmpty(textBoxJavaPath.Text);
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
            Settings.GoogleClosureOffline = chkEnableOfline.Checked;
            Settings.GoogleClosureJavaPath = textBoxJavaPath.Text;
            Settings.Save();
        }

        private void chkEnableOfline_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxOffline.Enabled = chkEnableOfline.Checked;
            if (string.IsNullOrEmpty(textBoxJavaPath.Text))
                FindJavaPath();
        }

        private void btnFindJava_Click(object sender, EventArgs e)
        {
            if (!FindJavaPath())
            {

                //active textbox if java path is not present in registry
                textBoxJavaPath.Enabled = true;

                //not found in registry, ask user to find java.exe
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
                    GotJavaPath(openFileDialog1.FileName);
                }
            }

        }

        private bool FindJavaPath() {
            //Try registry first
            var regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\JavaSoft\Java Runtime Environment")
                ??
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\JavaSoft\Java Runtime Environment");

            if (regKey != null)
            {
                var currentVersion = Convert.ToString(regKey.GetValue("CurrentVersion", string.Empty));
                if (!string.IsNullOrEmpty(currentVersion))
                {
                    if (GotJavaPath(Convert.ToString(regKey.OpenSubKey(currentVersion).GetValue("JavaHome", string.Empty)) + @"\bin\javaw.exe"))
                        return true;
                }
            }
            return false;
        }

        private bool GotJavaPath(string path)
        {
            if (System.IO.File.Exists(path))
            {
                textBoxJavaPath.Text = path;
                return true;
            }
            return false;
        }
    }
}
