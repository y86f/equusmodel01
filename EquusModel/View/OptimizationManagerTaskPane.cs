using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Tools;

namespace EquusModel.View
{
    public partial class OptimizationManagerTaskPane : UserControl
    {
        public OptimizationManagerTaskPane()
        {
            InitializeComponent();
        }

        private void OptimizationManagerTaskPane_Load(object sender, EventArgs e)
        {
            Control.SolverProperty sp = new Control.SolverProperty();

            propertyGrid1.SelectedObject = sp;
            Globals.ThisAddIn.AddPropperty(
                Globals.ThisAddIn.Application.ActiveWorkbook,
                sp);
        }

        private void OptimizationManagerTaskPane_SizeChanged(object sender, EventArgs e)
        {
            string wbName = Globals.ThisAddIn.Application.ActiveWorkbook.Name;
            if (Globals.ThisAddIn.TaskPanes.ContainsKey(wbName))
            {
                CustomTaskPane ctp = Globals.ThisAddIn.TaskPanes[wbName];
                if (ctp.DockPosition == Microsoft.Office.Core.MsoCTPDockPosition.msoCTPDockPositionRight
                    && ctp.Width <= 420)
                {
                    SendKeys.Send("{ESC}");
                    ctp.Width = 420;
                }
            }

        }
    }
}
