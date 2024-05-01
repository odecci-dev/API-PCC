using API_PCC.ApplicationModels.Common;

namespace API_PCC.ApplicationModels
{
    public class UserTypeSearchFilterModel
    {
        public string searchValue { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
    }
}
