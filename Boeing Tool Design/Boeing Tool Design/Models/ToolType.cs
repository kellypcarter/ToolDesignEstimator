namespace Boeing_Tool_Design.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ToolType")]
    public partial class ToolType
    {
        public int ToolTypeID { get; set; }

        [Required]
        [StringLength(10)]
        public string ToolCode { get; set; }

        public float? StandardFlow { get; set; }

        [StringLength(100)]
        public string ToolTypeName { get; set; }

        [StringLength(2000)]
        public string ToolTypeDescription { get; set; }

        public bool IsStressRequiredDefault { get; set; }

        public int? CreatedByUserID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? CreatedDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? UpdatedDate { get; set; }
    }
}
