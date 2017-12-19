using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Boeing_Tool_Design.Models;

namespace Boeing_Tool_Design.Models
{
    public class EstimatesViewModel
    {
        public List<DesignOrder> DesignOrders { get; set; }
        public List<Estimate> Estimates { get; set; }
        public List<AppUser> AppUsers { get; set; }
        public List<ToolType> ToolTypes { get; set; }
        public List<FamilyClass> FamilyClasses { get; set; }
        public List<StressWorkType> StressWorkTypes { get; set; }
        public List<Statistic> Statistics { get; set; }
        public List<ComplexityLevel> ComplexityLevels { get; set; }

        //public EstimatesViewModel()
        //{
        //    // Initialize all these as empty lists, so that they can quickly be added to in the actions
        //    this.DesignOrders = new List<DesignOrder>();
        //    this.Estimates = new List<Estimate>();
        //    this.AppUsers = new List<AppUser>();
        //    this.ToolTypes = new List<ToolType>();
        //    this.FamilyClasses = new List<FamilyClass>();
        //    this.StressWorkTypes = new List<StressWorkType>();
        //}
    }
}
