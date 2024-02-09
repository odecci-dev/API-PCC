using System;
using System.Collections.Generic;

namespace API_PCC.Models
{
    public partial class ActionTbl
    {
        public int ActionId { get; set; }
        public string ActionName { get; set; } = null!;
        public string ModuleId { get; set; } = null!;
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
