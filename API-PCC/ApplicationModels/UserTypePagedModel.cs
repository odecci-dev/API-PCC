using API_PCC.ApplicationModels.Common;
using API_PCC.EntityModels;

namespace API_PCC.ApplicationModels
{
    public class UserTypePagedModel : PaginationModel
    {
        public List<TblUserTypeModel> items { get; set; }
    }
}
