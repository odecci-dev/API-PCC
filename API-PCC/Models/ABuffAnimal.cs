using System;
using System.Collections.Generic;

namespace API_PCC.Models
{
    public partial class ABuffAnimal
    {
        public int Id { get; set; }
        public string AnimalId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Rfid { get; set; } = null!;
        public string HerdCode { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string Sex { get; set; } = null!;
        public string CountryBirth { get; set; } = null!;
        public string OriginAcquisition { get; set; } = null!;
        public DateTime DateAcquisition { get; set; }
        public string Marking { get; set; } = null!;
        public string SireRegNum { get; set; } = null!;
        public string SireIdNum { get; set; } = null!;
        public string BreedCode { get; set; } = null!;
        public string BloodCode { get; set; } = null!;
        public string BirthTypeCode { get; set; } = null!;
        public string TypeOwnCode { get; set; } = null!;
        public bool DeleteFlag { get; set; }
        public string CreatedBy { get; set; } = null!;
        public string UpdatedBy { get; set; } = null!;
        public DateTime? DateDelete { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DateRestored { get; set; }
        public string? RestoredBy { get; set; }
    }
}
