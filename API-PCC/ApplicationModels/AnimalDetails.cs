namespace API_PCC.ApplicationModels
{
    public class AnimalDetails
    {
        public DateTime? DateOfRegistration { get; set; }
        public string BreedRegistrationNumber { get; set; }
        public string HerdCode { get; set; }
        public string AnimalIdNumber { get; set; }
        public string Name { get; set; }
        public string Rfid { get; set; }
        public string Sex { get; set; }
        public string Breed { get; set; }
        public int? BloodComposition { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string CountryOfBirth { get; set; }
        public string BirthType { get; set; }
        public OriginOfAcquisitionModel OriginOfAcquisition { get; set; }
        public DateTime? DateOfAcquisition { get; set; }
        public string TypeOfOWnership { get; set; }
    }
}
