using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EquusModel.Models
{
    public class Food : EntityBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID {get;set;}
        [MaxLength]
        public string Description {get;set;}
    }
}
