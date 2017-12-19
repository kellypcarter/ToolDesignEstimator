namespace Boeing_Tool_Design.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ComplexityLevel")]
    public partial class ComplexityLevel
    {
        [Key]
        [Column("ComplexityLevel")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ComplexityLevelNum { get; set; }

        [Required]
        [StringLength(50)]
        public string ComplexityLevelName { get; set; }

        [StringLength(2000)]
        public string ComplexityLevelDescription { get; set; }

        public int? CreatedByUserID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? CreatedDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? UpdatedDate { get; set; }
    }
}
