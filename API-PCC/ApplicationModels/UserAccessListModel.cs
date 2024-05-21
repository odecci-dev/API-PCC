namespace API_PCC.ApplicationModels
{
    public class UserAccessListModel
    {
        public string username { get; set; }
        public Dictionary<string, List<int>> userAccessList { get; set; }

    }
}
