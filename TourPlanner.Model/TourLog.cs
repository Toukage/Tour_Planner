using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Model
{
    [Table("tourlog")]
    public class TourLog
    {
        [Key]
        [Column("tourlogid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LogID { get; set; }

        // FK -> Tour
        [Required]
        [Column("tourid")]
        public int TourID { get; set; }

        [Required]
        [Column("logdate", TypeName = "timestamp with time zone")]
        public DateTime LogDate { get; set; }

        [Column("logcomment")]
        public string? LogComment { get; set; }

        [Required]
        [Range(1, 5)]
        [Column("logdifficulty")]
        public int LogDifficulty { get; set; }

        [Required]
        [Column("logdistance")]
        public float LogDistance { get; set; }

        [Required]
        [Column("logtime")]
        public float LogTime { get; set; }

        [Required]
        [Range(1, 5)]
        [Column("rating")]
        public int Rating { get; set; } 
    }
}
