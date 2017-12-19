namespace Boeing_Tool_Design.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StressWorkType")]
    public partial class StressWorkType
    {
        public int StressWorkTypeID { get; set; }

        [Required]
        [StringLength(20)]
        public string StressWorkTypeName { get; set; }

        [StringLength(2000)]
        public string StressWorkTypeDescription { get; set; }

        public int? CreatedByUserID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? CreatedDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? UpdatedDate { get; set; }
    }
}
