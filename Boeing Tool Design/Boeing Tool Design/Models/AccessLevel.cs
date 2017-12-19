namespace Boeing_Tool_Design.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AccessLevel")]
    public partial class AccessLevel
    {
        public int AccessLevelID { get; set; }

        [Required]
        [StringLength(100)]
        public string AccessLevelName { get; set; }

        [Required]
        [StringLength(2000)]
        public string AccessLevelDescription { get; set; }

        public int? CreatedByUserID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? CreatedDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? UpdatedDate { get; set; }
    }
}
