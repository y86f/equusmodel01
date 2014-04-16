using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquusModel.Models
{
    public class NutrientCharacteristic : EntityBase
    {
        [Key,
        DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NutrientID { get; set; }
        public double MinNutrient { get; set; }
        public double MaxNutrient { get; set; }

    }
}
