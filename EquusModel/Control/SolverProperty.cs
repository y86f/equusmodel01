using System.ComponentModel;

/// <summary>
// Customer class to be displayed in the property grid
/// </summary>
/// 

namespace EquusModel.Control
{
    [DefaultPropertyAttribute("DatabaseLocation")]
    public class SolverProperty
    {
        private string _connectionString;
        private string _dataBasePath;
        private int _solveTime;
        private double _gap;

        [CategoryAttribute("Database Settigns"),
        DisplayName("Database Location"),
        DescriptionAttribute("Database that contains parameters to run the model."),
        EditorAttribute(typeof(System.Windows.Forms.Design.FileNameEditor),
            typeof(System.Drawing.Design.UITypeEditor))]
        public string DatabaseLocation
        {
            get
            {
                return _dataBasePath;
            }
            set
            {
                _dataBasePath = value;
                _connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;"
                + @"data source='" + value + "'";
            }
        }

        [CategoryAttribute("Database Settigns"),
        DisplayName("Connection String")]
        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
        }

        [CategoryAttribute("Solver Settings"),
        DescriptionAttribute("Maxium time of solving, in seconds, per run.")]
        public int SolveTime
        {
            get
            {
                return _solveTime;
            }
            set
            {
                _solveTime = value;
            }
        }

        [CategoryAttribute("Solver Settings"),
        DescriptionAttribute("Percentage from optimum solution enough to interrup solver.")]
        public double Gap
        {
            get
            {
                return _gap;
            }
            set
            {
                _gap = value;
            }
        }

        //constructor
        public SolverProperty()
        {
            _solveTime = (int)Properties.Settings.Default["SolveTime"];
            _gap = (double)Properties.Settings.Default["Gap"];
        }
    }
}