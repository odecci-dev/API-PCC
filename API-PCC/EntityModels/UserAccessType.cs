namespace API_PCC.EntityModels
{
    public class UserAccessType
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public int userAccessModelId { get; set; }
        public UserAccessModel accessModel { get; set; }
    }
}
