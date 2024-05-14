using API_PCC.Models;

namespace API_PCC.EntityModels
{
    public class BuffHerdJoinTable
    {
        public int BuffHerdJoinTableId { get; set; }
        public int BuffaloTypeId { get; set; }
        public int BuffHerdId { get; set; }
        public int FeedingSystemId { get; set; }
    }
}
