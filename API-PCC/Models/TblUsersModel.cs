﻿using System;
using System.Collections.Generic;

namespace API_PCC.Models
{
    public partial class TblUsersModel
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string Password { get; set; } = null!;
        public string? Fullname { get; set; }
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public string? Mname { get; set; }
        public string Email { get; set; } = null!;
        public string? Gender { get; set; }
        public string? EmployeeId { get; set; }
        public string? Jwtoken { get; set; }
        public string? FilePath { get; set; }
        public int? Active { get; set; }
        public DateTime? DateCreated { get; set; }
        public string? Cno { get; set; }
        public string? Address { get; set; }
        public string? Otp { get; set; }
        public int? Attempts { get; set; }
        public int? Status { get; set; }
        public DateTime DateCreated1 { get; set; }
        public DateTime DateUpdated { get; set; }
        public bool DeleteFlag { get; set; }
        public string CreatedBy { get; set; } = null!;
        public string UpdatedBy { get; set; } = null!;
        public DateTime? DateDelete { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DateRestored { get; set; }
        public string? RestoredBy { get; set; }
        public int? CenterId { get; set; }
        public string? Center { get; set; }
    }
}
