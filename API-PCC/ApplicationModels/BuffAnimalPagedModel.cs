﻿using API_PCC.ApplicationModels.Common;
using API_PCC.Models;

namespace API_PCC.ApplicationModels
{
    public class BuffAnimalPagedModel : PaginationModel
    {
        public List<ABuffAnimal> items { get; set; }

    }
}