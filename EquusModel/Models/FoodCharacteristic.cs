using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquusModel.Models
{
    public class FoodCharacteristic : EntityBase
    {
        public int ID { get; set; }
        public int FoodID { get; set; }
        public double Price { get; set; }
        public double MinServings { get; set; }
        public double MaxServings { get; set; }
    }
}
