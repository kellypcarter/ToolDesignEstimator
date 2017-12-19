namespace Boeing_Tool_Design.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DesignOrder")]
    public partial class DesignOrder
    {
        [Key]
        public int DesignOrderSID { get; set; }

        [Required(ErrorMessage = "Design Order Number is Required. If order number is unknown, please use a 1-2 word description.")]
        [Display(Name = "Design Order Number")]
        [StringLength(100)]
        public string DesignOrderNumber { get; set; }

        [Required]
        [Display(Name = "Descriptive Name")]
        [StringLength(500)]
        public string DescriptiveName { get; set; }

        [Required]
        public int ToolTypeID { get; set; }

        public string PartNumber {get; set; }

        [Display(Name = "Completed")]
        public bool IsCompleted { get; set; }

        [Display(Name = "Actual Hours")]
        public float? ActualHours { get; set; }

        public int? CreatedByUserID { get; set; }

        [Column(TypeName = "datetime2")]
        [Display(Name = "Date Created")]
        public DateTime? CreatedDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        /*
         * Print out the date formatted
         * */
        public string getFormattedCreatedDate ()
        {
            DateTime CreatedDate = this.CreatedDate ?? DateTime.Now;
            return CreatedDate.ToString("d", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
        }
    }
}
