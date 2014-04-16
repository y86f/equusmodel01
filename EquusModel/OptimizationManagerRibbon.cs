using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;

namespace EquusModel
{
    public partial class OptimizationManagerRibbon
    {
        private void OptimizationManagerRibbon_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void togglePanelButton_Click(object sender, RibbonControlEventArgs e)
        {
            bool visible = ((RibbonToggleButton)sender).Checked;
            string WorkbookName = Globals.ThisAddIn.Application.ActiveWorkbook.Name;
            Globals.ThisAddIn.TaskPanes[WorkbookName].Visible = visible;
        }
    }
}
