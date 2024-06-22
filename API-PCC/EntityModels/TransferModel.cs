using API_PCC.ApplicationModels;
using API_PCC.Models;
using System.Security.Policy;

namespace API_PCC.EntityModels
{
    public class TransferModel
    {
        public int Id { get; set; }
        public string transferNumber { get; set; }
        public int AnimalId { get; set; }
        public int OwnerId { get; set; }
        public virtual ABuffAnimal Animal { get; set; }
        public virtual TblFarmOwner Owner { get; set; }
        public string Address { get; set; }
        public string TelephoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Email {  get; set; }
        public byte[]? TransferFile { get; set; }
        public int Status { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public bool DeleteFlag { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? DateDeleted { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DateRestored { get; set; }
        public string? RestoredBy { get; set; }
    }
}
