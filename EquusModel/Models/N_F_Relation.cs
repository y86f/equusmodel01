using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquusModel.Models
{
    public class N_F_Relation : EntityBase
    {
        public int ID { get; set; }
        public int FoodID { get; set; }
        public int NutrientID { get; set; }
        public double NutrientPerFood { get; set; }
    }
}
