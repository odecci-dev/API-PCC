﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace API_PCC.Models;

public partial class HFeedingSystem
{
    public int Id { get; set; }

    public string FeedCode { get; set; }

    public string FeedDesc { get; set; }

    public int Status { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }

    public string UserId { get; set; }
}