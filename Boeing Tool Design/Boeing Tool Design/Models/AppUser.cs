namespace Boeing_Tool_Design.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    // Crypto Class
    using System.Web.Helpers;
    // Encoding
    using System.Text;

    [Table("AppUser")]
    public partial class AppUser
    {
        [Key]
        public int AppUserID { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string UserEmail { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string PasswordSalt { get; set; }

        public int AccessLevelID { get; set; }

        public int? CreatedByUserID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public string GetFullName()
        {
            return this.FirstName + " " + this.LastName;
        }
    }
}