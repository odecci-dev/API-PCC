namespace API_PCC.ApplicationModels
{
    public class BuffHerdViewResponseModel
    {
        public int id { get; set; }
        public string HerdName { get; set; }
        public string HerdClassDesc { get; set; }
        public string HerdClassCode { get; set; }
        public int HerdSize { get; set; }
        public string FarmManager { get; set; }
        public string HerdCode { get; set; }
        public List<string> BreedTypeCodeList { get; set; } = new List<String>();
        public string FarmAffilCode { get; set; }
        public List<string> FeedingSystemCodeList { get; set; } = new List<String>();
        public string FarmAddress { get; set; }
        public Owner Owner { get; set; }
        public int Status { get; set; }
        public string OrganizationName { get; set; }
        public string Center { get; set; }
        public string Photo { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public bool DeleteFlag { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DateDeleted { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DateRestored { get; set; }
        public string RestoredBy { get; set; }
    }
}
