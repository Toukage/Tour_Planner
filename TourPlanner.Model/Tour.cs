using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TourPlanner.Model
{
    [Table("tour")]
    public class Tour
    {
        [Key]
        [Column("tourid")]
        public int TourID { get; set; }

        [Required, StringLength(225)]
        [Column("tourname")]
        public string TourName { get; set; }

        [Required]
        [Column("description")]
        public string Description { get; set; }

        [Required, StringLength(50)]
        [Column("tourstart")]
        public string TourStart { get; set; }

        [Required, StringLength(50)]
        [Column("tourend")]
        public string TourEnd { get; set; }

        [Required, StringLength(50)]
        [Column("transport")]
        public string Transport { get; set; }

        [Column("distance")]
        public float? Distance { get; set; }

        [Column("esttime")]
        public float? EstTime { get; set; }

        [Column("routemap")]
        public string? RouteMap { get; set; }
    }
}
