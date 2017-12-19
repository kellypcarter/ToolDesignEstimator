namespace Boeing_Tool_Design.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FamilyClass")]
    public partial class FamilyClass
    {
        public int FamilyClassID { get; set; }

        [Required]
        [StringLength(256)]
        public string FamilyClassName { get; set; }

        [StringLength(2000)]
        public string FamilyClassDescription { get; set; }

        public int? CreatedByUserID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? CreatedDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? UpdatedDate { get; set; }
    }
}
