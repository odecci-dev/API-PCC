namespace API_PCC.ApplicationModels
{
    public class UserResponseModel
    {
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string Mname { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string EmployeeId { get; set; }
        public int? Active { get; set; }
        public string Cno { get; set; }
        public string Address { get; set; }
        public int? CenterId { get; set; }
        public int? Id { get; set; }
        public string? FilePath { get; set; }
        public bool? AgreementStatus { get; set; }
        public Dictionary<string, List<int>> userAccessList { get; set;}
    }
}
