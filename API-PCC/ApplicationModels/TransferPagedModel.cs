using API_PCC.ApplicationModels.Common;

namespace API_PCC.ApplicationModels
{
    public class TransferPagedModel : PaginationModel
    {
        public List<TransferResponseModel> items { get; set; }

    }
}
