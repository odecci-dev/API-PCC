using API_PCC.Models;

namespace API_PCC.EntityModels
{
    public class UserAccessModel
    {
        public int id { get; set; }

        public int userModelId { get; set; }
        public TblUsersModel userModel { get; set; }
        public string module { get; set; }

        public ICollection<UserAccessType>? userAccess { get; set; } = new List<UserAccessType>()!;

    }
}
