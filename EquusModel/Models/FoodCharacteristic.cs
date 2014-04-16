using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquusModel.Models
{
    public class FoodCharacteristic : EntityBase
    {
        [Key,
        DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FoodID { get; set; }
        public double Price { get; set; }
        public double MinServings { get; set; }
        public double MaxServings { get; set; }
    }
}
