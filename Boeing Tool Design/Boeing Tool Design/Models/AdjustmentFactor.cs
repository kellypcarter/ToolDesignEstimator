namespace Boeing_Tool_Design.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AdjustmentFactor")]
    public partial class AdjustmentFactor
    {
        public int AdjustmentFactorID { get; set; }

        public float? NSAR { get; set; }

        public float? NSRR { get; set; }

        public float? StressAnalysis { get; set; }

        public float? SurfacingRequired { get; set; }

        public float? EngineeringReleased { get; set; }

        public float? FirstToolInFamily { get; set; }

        public float? SecondToolInFamily { get; set; }

        public float? KBEDevelopment { get; set; }

        public float? KBEFollowOnTooling { get; set; }

        public int? CreatedByUserID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? CreatedDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? UpdatedDate { get; set; }
    }
}
