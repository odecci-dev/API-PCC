﻿using API_PCC.ApplicationModels.Common;
using API_PCC.Models;

namespace API_PCC.ApplicationModels
{
    public class HerdClassificationPagedModel : PaginationModel
    {
        public List<HHerdClassification> items { get; set; }

    }
}