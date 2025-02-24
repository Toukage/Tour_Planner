using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Model;
using Microsoft.EntityFrameworkCore;

namespace TourPlanner.Model
{
    [Table("Tour")]
    public class Tour
    {
        [Key]
        public int TourID { get; set; }

        [Required, StringLength(225)]
        public string TourName { get; set; }

        [Required]
        public string Description { get; set; }

        [Required, StringLength(50)]
        public string TourStart { get; set; }

        [Required, StringLength(50)]
        public string TourEnd { get; set; }

        [Required, StringLength(50)]
        public string Transport { get; set; }

        public float? Distance { get; set; }
        public float? EstTime { get; set; }
        public string RouteMap { get; set; }
    }
}
