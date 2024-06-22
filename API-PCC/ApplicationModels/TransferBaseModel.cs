using System.ComponentModel.DataAnnotations;

namespace API_PCC.ApplicationModels
{
    public class TransferBaseModel
    {
        public string BreedRegistrationNumber { get; set; }
        public string AnimalIdNumber { get; set; }
        public string Owner { get; set; }
        public string Address { get; set; }
        public string TelephoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
    }
}
