namespace API_PCC.EntityModels
{
    public class TblUserTypeModel
    {
        public int Id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }
        public bool? DeleteFlag { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? DateDeleted { get; set; }

        public string DeletedBy { get; set; }

        public DateTime? DateRestored { get; set; }

        public string RestoredBy { get; set; }
    }
}
