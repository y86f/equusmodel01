﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquusModel.Models
{
    public class V_FoodQuantity
    {
        [Key,
        DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FoodID { get; set; }
        public double Quantity { get; set; }
    }
}
