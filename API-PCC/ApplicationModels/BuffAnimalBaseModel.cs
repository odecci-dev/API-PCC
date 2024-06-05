namespace API_PCC.ApplicationModels
{
    public class BuffAnimalBaseModel
    {
        public int Id { get; set; }
        public string AnimalIdNumber { get; set; }
        public string AnimalName { get; set; }
        public String Photo { get; set; }
        public string HerdCode { get; set; }
        public string RfidNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Sex { get; set; }
        public string BreedCode { get; set; }
        public string BirthType { get; set; }
        public string CountryOfBirth { get; set; }
        public OriginOfAcquisitionModel OriginOfAcquisition { get; set; }
        public DateTime? DateOfAcquisition { get; set; }
        public string Marking { get; set; }
        public string TypeOfOwnership { get; set; }
        public string BloodCode { get; set; }
        public Animal Sire { get; set; }
        public Animal Dam { get; set; }
    }
}
