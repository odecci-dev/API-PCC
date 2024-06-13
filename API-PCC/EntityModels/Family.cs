using API_PCC.ApplicationModels;
using API_PCC.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_PCC.EntityModels
{
    public class Family
    {
        public int Id { get; set; }
        public int animalId { get; set; }
        public virtual ABuffAnimal sire { get; set; }
        public virtual ABuffAnimal dam { get; set; }
        public int sireId { get; set; }
        public int damId { get; set; }
        public int status { get; set; }
        public bool deleteFlag { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? DateDeleted { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DateRestored { get; set; }
        public string? RestoredBy { get; set; }
    }
}
