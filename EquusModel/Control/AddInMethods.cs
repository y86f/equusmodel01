using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Office.Tools;
using Excel = Microsoft.Office.Interop.Excel;
using EquusModel.Models;
using System.Diagnostics;
using System;

namespace EquusModel.Control
{
    /* COM INTERFACE IMPLEMENTATION*/
    [ComVisible(true)]
    public interface IAddinMethods
    {
        void solveModel();
    }
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class AddInMethods : IAddinMethods
    {
        //Solves classic Diet Model Optimization Problem
        public void solveModel()
        {
            Excel.Workbook wb = 
                Globals.ThisAddIn.Application.ActiveWorkbook;
            if (Globals.ThisAddIn.Properties.ContainsKey(wb.Name))
            {
                OptimizationModel.Solve(Globals.ThisAddIn.Properties[wb.Name], wb);
            }
            else
            {
                MessageBox.Show(
                    "Workbook not validated to run the model.", 
                    "Equus Optimization Manager");
            }
        }
    }
}
