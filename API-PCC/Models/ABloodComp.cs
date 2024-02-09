using System;
using System.Collections.Generic;

namespace API_PCC.Models
{
    public partial class ABloodComp
    {
        public int Id { get; set; }
        public string BloodCode { get; set; } = null!;
        public string BloodDesc { get; set; } = null!;
        public int Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public bool DeleteFlag { get; set; }
        public string CreatedBy { get; set; } = null!;
        public string UpdatedBy { get; set; } = null!;
        public DateTime? DateDelete { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DateRestored { get; set; }
        public string? RestoredBy { get; set; }
    }
}
