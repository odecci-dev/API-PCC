namespace API_PCC.Models
{
    public class TblTokenModel
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }

    }
}
