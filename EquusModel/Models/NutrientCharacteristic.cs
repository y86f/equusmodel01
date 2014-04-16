using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquusModel.Models
{
    public class NutrientCharacteristic : EntityBase
    {
        public int ID { get; set; }
        public int NutrientID { get; set; }
        public double MinNutrient { get; set; }
        public double MaxNutrient { get; set; }

    }
}
