namespace Boeing_Tool_Design.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Statistic")]
    public partial class Statistic
    {
        public int StatisticID { get; set; }

        [StringLength(2000)]
        public string StatisticNotes { get; set; }

        public int ToolTypeID { get; set; }

        public int ComplexityLevel { get; set; }

        public double StandardDeviation { get; set; }

        public float AverageHours { get; set; }

        public bool isCurrent { get; set; }

        [StringLength(200)]
        public string ImageFilePath { get; set; }

        public float ReleaseHours { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime EffectiveStartDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? EffectiveEndDate { get; set; }

        public int? CreatedByUserID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? CreatedDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? UpdatedDate { get; set; }
    }
}
