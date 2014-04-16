using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;
using Microsoft.Office.Tools;
using System.Windows.Forms;
using EquusModel.Models;
using EquusModel.Control;

namespace EquusModel
{
    public partial class ThisAddIn
    {
        private AddInMethods addInMethods;
        private Dictionary<string, View.OptimizationManagerTaskPane> _modelTaskPanes;
        private Dictionary<string, CustomTaskPane> _customTaskPanes;
        private Dictionary<string, SolverProperty> _solverProperty;

        private void AddTaskPane(Excel.Workbook Wb)
        {
            View.OptimizationManagerTaskPane _omTaskPane1 = 
                new View.OptimizationManagerTaskPane();

            CustomTaskPane _customTaskPane =
                this.CustomTaskPanes.Add(
                    _omTaskPane1,
                    "CBMP Setup Options",
                    Wb.Application.ActiveWindow);
            _customTaskPane.Visible = false;
            _customTaskPane.VisibleChanged +=
                new EventHandler(customTaskPaneValue_VisibleChanged);

            _modelTaskPanes.Add(Wb.Name, _omTaskPane1);
            _customTaskPanes.Add(Wb.Name, _customTaskPane);
        }
        private void RemoveTaskPane(Excel.Workbook Wb)
        {
            if (_customTaskPanes[Wb.Name] != null)
            {
                _customTaskPanes[Wb.Name].Dispose();
                _customTaskPanes.Remove(Wb.Name);
            }
            if (_modelTaskPanes[Wb.Name] != null)
            {
                _modelTaskPanes[Wb.Name].Dispose();
                _modelTaskPanes.Remove(Wb.Name);
            }
        }
        public void AddPropperty(Excel.Workbook Wb, SolverProperty sp)
        {
            if (_solverProperty.ContainsKey(Wb.Name))
            {
                _solverProperty[Wb.Name] = sp;
            }
            else
            {
                _solverProperty.Add(Wb.Name, sp);
            }
        }
        // exposes cutomTaskPane to other classes
        public Dictionary<string, CustomTaskPane> TaskPanes
        {
            get
            {
                if (_customTaskPanes == null)
                {
                    _customTaskPanes = new Dictionary<string, CustomTaskPane>();
                }
                return _customTaskPanes;
            }
        }

        public Dictionary<string, SolverProperty> Properties
        {
            get
            {
                if (_solverProperty == null)
                {
                    _solverProperty = new Dictionary<string, SolverProperty>();
                }
                return _solverProperty;
            }
        }
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            // initiate connection String
            _solverProperty = new Dictionary<string, SolverProperty>();
            _customTaskPanes = new Dictionary<string, CustomTaskPane>();
            _modelTaskPanes = new Dictionary<string, View.OptimizationManagerTaskPane>();

            // adds events to applications
            this.Application.WorkbookBeforeSave +=
                new Excel.AppEvents_WorkbookBeforeSaveEventHandler(ThisAddIn_BeforeSave);
            this.Application.WorkbookOpen +=
                new Excel.AppEvents_WorkbookOpenEventHandler(ThisAddIn_Open);
            ((Excel.AppEvents_Event)this.Application).NewWorkbook +=
                new Excel.AppEvents_NewWorkbookEventHandler(ThisWorkbook_NewWorkbook);
            this.Application.WorkbookBeforeClose +=
                new Excel.AppEvents_WorkbookBeforeCloseEventHandler(ThisAddIn_BeforeClose);
            this.Application.WindowActivate +=
                new Excel.AppEvents_WindowActivateEventHandler(ThisAddin_WindowActivate);
        }
        private void ThisAddin_WindowActivate(Excel.Workbook Wb, Excel.Window Wn)
        {
            Globals.Ribbons.OptimizationManagerRibbon.togglePanelButton.Checked =
                _customTaskPanes[Wb.Name].Visible;
        }
        private void ThisAddIn_BeforeClose(Excel.Workbook Wb, ref bool Cancel)
        {
            RemoveTaskPane(Wb);
        }
        private void ThisWorkbook_NewWorkbook(Excel.Workbook Wb)
        {
            // initiate task pane for options
            AddTaskPane(Wb);
        }
        private void ThisAddIn_BeforeSave(Excel.Workbook Wb, bool SaveAsUI, ref bool Cancel)
        {
            /*Excel.Worksheet wsThis = (Excel.Worksheet)this.Application.ActiveSheet;

            ((Excel.Range)wsThis.get_Range("A1")).Value2 = "Test from Add in";*/
        }
        private void ThisAddIn_Open(Excel.Workbook Wb)
        {
            // initiate task pane for options
            AddTaskPane(Wb);
        }
        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        private void customTaskPaneValue_VisibleChanged(object sender, System.EventArgs e)
        {
            bool visible = ((CustomTaskPane)sender).Visible;
            Globals.Ribbons.OptimizationManagerRibbon.togglePanelButton.Checked = visible;
        }
        protected override object RequestComAddInAutomationService()
        {
            if (addInMethods == null)
            {
                addInMethods = new AddInMethods();
            }
            return addInMethods;
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
