﻿using API_PCC.ApplicationModels.Common;
using API_PCC.Models;

namespace API_PCC.ApplicationModels
{
    public class FeedingSystemPagedModel : PaginationModel
    {
        public List<HFeedingSystem> items { get; set; }

    }
}