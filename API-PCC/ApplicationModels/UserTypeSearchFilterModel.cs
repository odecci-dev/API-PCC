using API_PCC.ApplicationModels.Common;

namespace API_PCC.ApplicationModels
{
    public class UserTypeSearchFilterModel
    {
        public string searchParam { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
        public SortByModel sortBy { get; set; }

    }
}
