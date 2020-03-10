using Delini8.DataLayer.Profile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Delini8
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void cmdCount_Click(object sender, EventArgs e)
        {
            string sTopLevelPath = Administrator.ProfileManager.SystemProfile.SourcePath;
            long nEstimate = 0;
            Dir oDir = new Dir();
            txtLines.Text = "0";
            ArrayList oDirs = new ArrayList();
            oDir.DirList(sTopLevelPath, ref nEstimate);
            txtLines.Text = oDir.LineCount.ToString();
            Administrator.Tracer.WriteLine();
            string message = "Index".PadRight(17, ' ');
            foreach (KeyValuePair<int, string> group in oDir.GroupNames)
            {
                message += "," + group.Value.PadRight(7, ' ');
            }
            Administrator.Tracer.WriteLine(message);
            foreach (KeyValuePair<int, string> name in oDir.CounterNames)
            {
                message = string.Empty;
                foreach (KeyValuePair<int, string> group in oDir.GroupNames)
                {
                    if (group.Value == "All")
                    {
                        message += name.Value.PadRight(15, ' ');
                        message += "  ";
                    }
                    var groupCounters = oDir.Counters[group.Value];
                    long number = 0;
                    if (groupCounters.ContainsKey(name.Value))
                    {
                        number = groupCounters[name.Value];
                    }
                    message += "," + number.ToString().Trim().PadLeft(7, ' ');
                }
                Administrator.Tracer.WriteLine(message);
            }
            Administrator.Tracer.WriteLine();
            //foreach (KeyValuePair<string, string> ext in oDir.Exts)
            //{
            //    Administrator.Tracer.WriteLine(ext.Key);
            //}
            Administrator.View();
        }
    }
}