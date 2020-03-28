using Delini8.DataLayer.Profile;
using Delini8.DataLayer.Tracing;
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
            long nEstimate = 0;
            Dir oDir = new Dir();
            txtLines.Text = "0";
            ArrayList oDirs = new ArrayList();
            oDir.DirList(Administrator.ProfileManager.SystemProfile.SourcePath, ref nEstimate);
            var allCounters = oDir.Counters["All"];
            long totalLines = 0;
            if (allCounters.ContainsKey("TotalLines"))
            {
                totalLines = allCounters["TotalLines"];
            }
            txtLines.Text = totalLines.ToString();
            Administrator.Tracer.WriteLine();
            string message = "Index".PadRight(17, ' ');
            foreach (KeyValuePair<int, string> group in oDir.GroupNames)
            {
                message += "," + group.Value.PadLeft(7, ' ');
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
            //Administrator.Tracer.Outcome();
            //Administrator.Tracer.Finish();
            Administrator.View();
        }
    }
}