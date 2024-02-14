﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace API_PCC.Models;

public partial class ABuffAnimal
{
    public int Id { get; set; }

    public string AnimalId { get; set; }

    public string Name { get; set; }

    public string Rfid { get; set; }

    public string HerdCode { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string Sex { get; set; }

    public string CountryBirth { get; set; }

    public string OriginAcquisition { get; set; }

    public DateTime DateAcquisition { get; set; }

    public string Marking { get; set; }

    public string SireRegNum { get; set; }

    public string SireIdNum { get; set; }

    public string BreedCode { get; set; }

    public string BloodCode { get; set; }

    public string BirthTypeCode { get; set; }

    public string TypeOwnCode { get; set; }

    public bool DeleteFlag { get; set; }

    public string CreatedBy { get; set; }

    public string UpdatedBy { get; set; }

    public DateTime? DateDelete { get; set; }

    public string DeletedBy { get; set; }

    public DateTime? DateRestored { get; set; }

    public string RestoredBy { get; set; }
}