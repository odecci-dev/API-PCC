using System;
using System.Collections.Generic;

namespace API_PCC.Models
{
    public partial class HBuffHerd
    {
        public int Id { get; set; }
        public string HerdName { get; set; } = null!;
        public string HerdCode { get; set; } = null!;
        public int HerdSize { get; set; }
        public string BBuffCode { get; set; } = null!;
        public string FCode { get; set; } = null!;
        public string HTypeCode { get; set; } = null!;
        public string FeedCode { get; set; } = null!;
        public string FarmManager { get; set; } = null!;
        public string FarmAddress { get; set; } = null!;
        public string Owner { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string TelNo { get; set; } = null!;
        public string MNo { get; set; } = null!;
        public string Email { get; set; } = null!;
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
