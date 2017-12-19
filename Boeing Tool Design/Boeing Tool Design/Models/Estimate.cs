namespace Boeing_Tool_Design.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Estimate")]
    public partial class Estimate
    {
        public int EstimateID { get; set; }

        public int DesignOrderSID { get; set; }

        public bool NeedsSurfacing { get; set; }

        [Required]
        public bool IsStressIncluded { get; set; }

        public int StressWorkTypeID { get; set; }

        public int ComplexityLevel { get; set; }

        public bool EngineeringReleased { get; set; }

        public int? FamilyClassID { get; set; }

        [Display(Name = "Total Hours")]
        public float HoursEstimate { get; set; }

        public float? DesignHour { get; set; }

        public float? StressHour { get; set; }

        public float? CheckHour { get; set; }

        public float? ReleaseHour { get; set; }

        public float? DesignFlow { get; set; }

        // Added so that the estimate can track how many designs from the KBE are needed to break even
        public float? KBEBreakEvenNum { get; set; }

        public bool IsLatestEstimate { get; set; }

        [StringLength(2000)]
        public string Comment { get; set; }

        [Required]
        [StringLength(2000)]
        [Display(Name = "Reason For Estimate Change")]
        public string ReasonForEstimateChange { get; set; }

        public int? CreatedByUserID { get; set; }

        [Column(TypeName = "datetime2")]
        [Display(Name = "Date Made")]
        public DateTime? CreatedDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        /*
         * Print out the date formatted
         * */
        public string getFormattedCreatedDate()
        {
            DateTime CreatedDate = this.CreatedDate ?? DateTime.Now;
            return CreatedDate.ToString("d", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
        }
    }
}