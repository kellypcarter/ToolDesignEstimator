using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Boeing_Tool_Design.Models;

namespace Boeing_Tool_Design.Models
{
    public class AdminViewModel
    {
        public List<ToolType> ToolTypes { get; set; }
        public List<ComplexityLevel> ComplexityLevels { get; set; }
        public List<Statistic> Statistics { get; set; }
        //public List<AdjustmentFactor> AdjustmentFactors { get; set; }
        public AdjustmentFactor AdjustFactor { get; set; }
        public List<AppUser> AppUsers { get; set; }

    }
}
