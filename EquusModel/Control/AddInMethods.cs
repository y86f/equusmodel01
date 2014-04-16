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
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int k=0;
            for (int i = 1; i <= 10000; i++)
                for (int j = 1; j <= 1000; j++)
                    k = i + j;
            sw.Stop();
            Console.WriteLine("Elapsed: {0} seconds", sw.Elapsed);
            //TransportSample.Run();
            Excel.Workbook wb = Globals.ThisAddIn.Application.ActiveWorkbook;
            if (Globals.ThisAddIn.Properties.ContainsKey(wb.Name))
            {
                //TransportSample.Run();
                OptimizationModel.Solve(Globals.ThisAddIn.Properties[wb.Name]);
            }
            else
            {
                MessageBox.Show("Connection String not set.", "CBMP Model");
            }
        }
    }
}
