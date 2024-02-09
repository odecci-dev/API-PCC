using System;
using System.Collections.Generic;

namespace API_PCC.Models
{
    public partial class ModuleTbl
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = null!;
        public string ParentModule { get; set; } = null!;
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? DateUpdated { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? DateRestored { get; set; }
        public string? RestoredBy { get; set; }
        public DateTime? DateDelete { get; set; }
        public string? DeletedBy { get; set; }
        public bool DeleteFlag { get; set; }
    }
}
