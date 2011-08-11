using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Zippy.Chirp.ConfigurationScreen
{
    public partial class JSHint : UserControl, EnvDTE.IDTToolsOptionsPage
    {
        public JSHint()
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
            this.chkBitwise.Checked = Settings.JsHintOptions.bitwise;
            this.chkBoss.Checked = Settings.JsHintOptions.boss;
            this.chkCurly.Checked = Settings.JsHintOptions.curly;
            this.chkDebug.Checked = Settings.JsHintOptions.debug;
            //this.chkDevel.Checked = Settings.JsHintOptions.devel;
            this.chkEqeqeq.Checked = Settings.JsHintOptions.eqeqeq;
            this.chkEvil.Checked = Settings.JsHintOptions.evil;
            this.chkForin.Checked = Settings.JsHintOptions.forin;
            this.chkImmed.Checked = Settings.JsHintOptions.immed;
            this.chkLaxbreak.Checked = Settings.JsHintOptions.laxbreak;
            if (Settings.JsHintOptions.maxerr.HasValue)
            {
                this.TxtMaxerr.Value = Settings.JsHintOptions.maxerr.Value;
            }
            this.chkNewcap.Checked = Settings.JsHintOptions.newcapp;
            this.chkNoArg.Checked = Settings.JsHintOptions.noarg;
            this.chkNoEmpty.Checked = Settings.JsHintOptions.noempty;
            this.chkNomen.Checked = Settings.JsHintOptions.nomen;
            this.chkNoNew.Checked = Settings.JsHintOptions.nonew;
            this.chkNoVar.Checked = Settings.JsHintOptions.novar;
            this.chkPassfail.Checked = Settings.JsHintOptions.passfail;
            this.chkPlusPlus.Checked = Settings.JsHintOptions.plusplus;
            this.chkRegex.Checked = Settings.JsHintOptions.regex;
            this.chkStrict.Checked = Settings.JsHintOptions.strict;
            this.chkSub.Checked = Settings.JsHintOptions.sub;
            this.chkUndef.Checked = Settings.JsHintOptions.undef;
            this.chkWhite.Checked = Settings.JsHintOptions.white;
            this.chkJSHint.Checked = Settings.RunJSHint;

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
            Settings.JsHintOptions.devel = true;
            Settings.JsHintOptions.bitwise = this.chkBitwise.Checked;
            Settings.JsHintOptions.boss = this.chkBoss.Checked;
            Settings.JsHintOptions.curly = this.chkCurly.Checked;
            Settings.JsHintOptions.debug = this.chkDebug.Checked;
            //this.chkDevel.Checked = Settings.JsHintOptions.devel;
            Settings.JsHintOptions.eqeqeq = this.chkEqeqeq.Checked;
            Settings.JsHintOptions.evil = this.chkEvil.Checked;
            Settings.JsHintOptions.forin = this.chkForin.Checked;
            Settings.JsHintOptions.immed = this.chkImmed.Checked;
            Settings.JsHintOptions.laxbreak = this.chkLaxbreak.Checked;
            Settings.JsHintOptions.maxerr = (int)this.TxtMaxerr.Value;
            Settings.JsHintOptions.newcapp = this.chkNewcap.Checked;
            Settings.JsHintOptions.noarg=this.chkNoArg.Checked;
            Settings.JsHintOptions.noempty = this.chkNoEmpty.Checked;
            Settings.JsHintOptions.nomen = this.chkNomen.Checked;
            Settings.JsHintOptions.nonew = this.chkNoNew.Checked;
            Settings.JsHintOptions.novar = this.chkNoVar.Checked;
            Settings.JsHintOptions.passfail=this.chkPassfail.Checked;
            Settings.JsHintOptions.plusplus=this.chkPlusPlus.Checked;
            Settings.JsHintOptions.regex=this.chkRegex.Checked;
            Settings.JsHintOptions.strict=this.chkStrict.Checked;
            Settings.JsHintOptions.sub=this.chkSub.Checked;
            Settings.JsHintOptions.undef=this.chkUndef.Checked;
            Settings.JsHintOptions.white=this.chkWhite.Checked;
            this.chkJSHint.Checked = Settings.RunJSHint;
            Settings.Save();
        }

    }
}
