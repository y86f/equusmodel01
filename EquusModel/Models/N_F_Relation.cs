using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquusModel.Models
{
    public class N_F_Relation : EntityBase
    {
        [Key, Column(Order = 1),
        DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FoodID { get; set; }
        [Key, Column(Order = 2),
        DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NutrientID { get; set; }
        public double NutrientPerFood { get; set; }
    }
}
