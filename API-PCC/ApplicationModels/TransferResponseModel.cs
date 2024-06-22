using API_PCC.EntityModels;
using API_PCC.Models;

namespace API_PCC.ApplicationModels
{
    public class TransferResponseModel
    {
        public string transferNumber { get; set; }
        public string BreedRegistrationNumber { get; set; }
        public string AnimalIdNumber { get; set; }
        public string Owner {  get; set; }
        public string Address { get; set; }
        public string TelephoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public byte[]? TransferFile { get; set; }

    }
}
