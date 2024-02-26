﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using API_PCC.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace API_PCC.Data
{
    public partial class PCC_DEVContext
    {
        private IPCC_DEVContextProcedures _procedures;

        public virtual IPCC_DEVContextProcedures Procedures
        {
            get
            {
                if (_procedures is null) _procedures = new PCC_DEVContextProcedures(this);
                return _procedures;
            }
            set
            {
                _procedures = value;
            }
        }

        public IPCC_DEVContextProcedures GetProcedures()
        {
            return Procedures;
        }

        protected void OnModelCreatingGeneratedProcedures(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GetUserLogInResult>().HasNoKey().ToView(null);
        }
    }

    public partial class PCC_DEVContextProcedures : IPCC_DEVContextProcedures
    {
        private readonly PCC_DEVContext _context;

        public PCC_DEVContextProcedures(PCC_DEVContext context)
        {
            _context = context;
        }

        public virtual async Task<List<GetUserLogInResult>> GetUserLogInAsync(string Username, string Password, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default)
        {
            var parameterreturnValue = new SqlParameter
            {
                ParameterName = "returnValue",
                Direction = System.Data.ParameterDirection.Output,
                SqlDbType = System.Data.SqlDbType.Int,
            };

            var sqlParameters = new []
            {
                new SqlParameter
                {
                    ParameterName = "Username",
                    Size = 255,
                    Value = Username ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.VarChar,
                },
                new SqlParameter
                {
                    ParameterName = "Password",
                    Size = 255,
                    Value = Password ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.VarChar,
                },
                parameterreturnValue,
            };
            var _ = await _context.SqlQueryAsync<GetUserLogInResult>("EXEC @returnValue = [dbo].[GetUserLogIn] @Username, @Password", sqlParameters, cancellationToken);

            returnValue?.SetValue(parameterreturnValue.Value);

            return _;
        }
    }
}