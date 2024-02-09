using System;
using System.Collections.Generic;
using API_PCC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace API_PCC.Data
{
    public partial class PCC_DEVContext : DbContext
    {
        public PCC_DEVContext()
        {
        }

        public PCC_DEVContext(DbContextOptions<PCC_DEVContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ABirthType> ABirthTypes { get; set; } = null!;
        public virtual DbSet<ABloodComp> ABloodComps { get; set; } = null!;
        public virtual DbSet<ABreed> ABreeds { get; set; } = null!;
        public virtual DbSet<ABuffAnimal> ABuffAnimals { get; set; } = null!;
        public virtual DbSet<ATypeOwnership> ATypeOwnerships { get; set; } = null!;
        public virtual DbSet<ActionTbl> ActionTbls { get; set; } = null!;
        public virtual DbSet<HBuffHerd> HBuffHerds { get; set; } = null!;
        public virtual DbSet<HBuffaloType> HBuffaloTypes { get; set; } = null!;
        public virtual DbSet<HFarmerAffiliation> HFarmerAffiliations { get; set; } = null!;
        public virtual DbSet<HFeedingSystem> HFeedingSystems { get; set; } = null!;
        public virtual DbSet<HHerdType> HHerdTypes { get; set; } = null!;
        public virtual DbSet<ModuleTbl> ModuleTbls { get; set; } = null!;
        public virtual DbSet<TblApiTokenModel> TblApiTokenModels { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=ConnectionStrings:DevConnection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ABirthType>(entity =>
            {
                entity.ToTable("A_Birth_Type");

                entity.Property(e => e.BirthTypeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Birth_Type_Code");

                entity.Property(e => e.BirthTypeDesc)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Birth_Type_Desc");

                entity.Property(e => e.CreatedBy)
                    .IsUnicode(false)
                    .HasColumnName("Created_By");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("date")
                    .HasColumnName("Date_Created");

                entity.Property(e => e.DateDelete)
                    .HasColumnType("date")
                    .HasColumnName("Date_Delete");

                entity.Property(e => e.DateRestored)
                    .HasColumnType("date")
                    .HasColumnName("Date_Restored");

                entity.Property(e => e.DateUpdated)
                    .HasColumnType("date")
                    .HasColumnName("Date_Updated");

                entity.Property(e => e.DeleteFlag).HasColumnName("Delete_Flag");

                entity.Property(e => e.DeletedBy)
                    .IsUnicode(false)
                    .HasColumnName("Deleted_By");

                entity.Property(e => e.RestoredBy)
                    .IsUnicode(false)
                    .HasColumnName("Restored_By");

                entity.Property(e => e.UpdatedBy)
                    .IsUnicode(false)
                    .HasColumnName("Updated_By");
            });

            modelBuilder.Entity<ABloodComp>(entity =>
            {
                entity.ToTable("A_Blood_Comp");

                entity.Property(e => e.BloodCode)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasColumnName("Blood_Code");

                entity.Property(e => e.BloodDesc)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasColumnName("Blood_Desc");

                entity.Property(e => e.CreatedBy)
                    .IsUnicode(false)
                    .HasColumnName("Created_By");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("date")
                    .HasColumnName("Date_Created");

                entity.Property(e => e.DateDelete)
                    .HasColumnType("date")
                    .HasColumnName("Date_Delete");

                entity.Property(e => e.DateRestored)
                    .HasColumnType("date")
                    .HasColumnName("Date_Restored");

                entity.Property(e => e.DateUpdated)
                    .HasColumnType("date")
                    .HasColumnName("Date_Updated");

                entity.Property(e => e.DeleteFlag).HasColumnName("Delete_Flag");

                entity.Property(e => e.DeletedBy)
                    .IsUnicode(false)
                    .HasColumnName("Deleted_By");

                entity.Property(e => e.RestoredBy)
                    .IsUnicode(false)
                    .HasColumnName("Restored_By");

                entity.Property(e => e.UpdatedBy)
                    .IsUnicode(false)
                    .HasColumnName("Updated_By");
            });

            modelBuilder.Entity<ABreed>(entity =>
            {
                entity.ToTable("A_Breed");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BreedCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Breed_Code");

                entity.Property(e => e.BreedDesc)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Breed_Desc");

                entity.Property(e => e.CreatedBy)
                    .IsUnicode(false)
                    .HasColumnName("Created_By");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("date")
                    .HasColumnName("Date_Created");

                entity.Property(e => e.DateDelete)
                    .HasColumnType("date")
                    .HasColumnName("Date_Delete");

                entity.Property(e => e.DateRestored)
                    .HasColumnType("date")
                    .HasColumnName("Date_Restored");

                entity.Property(e => e.DateUpdated)
                    .HasColumnType("date")
                    .HasColumnName("Date_Updated");

                entity.Property(e => e.DeleteFlag).HasColumnName("Delete_Flag");

                entity.Property(e => e.DeletedBy)
                    .IsUnicode(false)
                    .HasColumnName("Deleted_By");

                entity.Property(e => e.RestoredBy)
                    .IsUnicode(false)
                    .HasColumnName("Restored_By");

                entity.Property(e => e.UpdatedBy)
                    .IsUnicode(false)
                    .HasColumnName("Updated_By");
            });

            modelBuilder.Entity<ABuffAnimal>(entity =>
            {
                entity.ToTable("A_Buff_Animal");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AnimalId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Animal_ID");

                entity.Property(e => e.BirthTypeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Birth_Type_Code");

                entity.Property(e => e.BloodCode)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasColumnName("Blood_Code");

                entity.Property(e => e.BreedCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("Breed_Code");

                entity.Property(e => e.CountryBirth)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Country_Birth");

                entity.Property(e => e.CreatedBy)
                    .IsUnicode(false)
                    .HasColumnName("Created_By");

                entity.Property(e => e.DateAcquisition)
                    .HasColumnType("date")
                    .HasColumnName("Date_Acquisition");

                entity.Property(e => e.DateDelete)
                    .HasColumnType("date")
                    .HasColumnName("Date_Delete");

                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("date")
                    .HasColumnName("Date_of_Birth");

                entity.Property(e => e.DateRestored)
                    .HasColumnType("date")
                    .HasColumnName("Date_Restored");

                entity.Property(e => e.DeleteFlag).HasColumnName("Delete_Flag");

                entity.Property(e => e.DeletedBy)
                    .IsUnicode(false)
                    .HasColumnName("Deleted_By");

                entity.Property(e => e.HerdCode)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("Herd_Code");

                entity.Property(e => e.Marking).IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OriginAcquisition)
                    .IsUnicode(false)
                    .HasColumnName("Origin_Acquisition");

                entity.Property(e => e.RestoredBy)
                    .IsUnicode(false)
                    .HasColumnName("Restored_By");

                entity.Property(e => e.Rfid)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("RFID");

                entity.Property(e => e.Sex)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.SireIdNum)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Sire_ID_Num");

                entity.Property(e => e.SireRegNum)
                    .HasMaxLength(17)
                    .IsUnicode(false)
                    .HasColumnName("Sire_Reg_Num");

                entity.Property(e => e.TypeOwnCode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Type_Own_Code");

                entity.Property(e => e.UpdatedBy)
                    .IsUnicode(false)
                    .HasColumnName("Updated_By");
            });

            modelBuilder.Entity<ATypeOwnership>(entity =>
            {
                entity.ToTable("A_Type_Ownership");

                entity.Property(e => e.CreatedBy)
                    .IsUnicode(false)
                    .HasColumnName("Created_By");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("date")
                    .HasColumnName("Date_Created");

                entity.Property(e => e.DateDelete)
                    .HasColumnType("date")
                    .HasColumnName("Date_Delete");

                entity.Property(e => e.DateRestored)
                    .HasColumnType("date")
                    .HasColumnName("Date_Restored");

                entity.Property(e => e.DateUpdated)
                    .HasColumnType("date")
                    .HasColumnName("Date_Updated");

                entity.Property(e => e.DeleteFlag).HasColumnName("Delete_Flag");

                entity.Property(e => e.DeletedBy)
                    .IsUnicode(false)
                    .HasColumnName("Deleted_By");

                entity.Property(e => e.RestoredBy)
                    .IsUnicode(false)
                    .HasColumnName("Restored_By");

                entity.Property(e => e.TypeOwnCode)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("Type_Own_Code");

                entity.Property(e => e.TypeOwnDesc)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("Type_Own_Desc");

                entity.Property(e => e.UpdatedBy)
                    .IsUnicode(false)
                    .HasColumnName("Updated_By");
            });

            modelBuilder.Entity<ActionTbl>(entity =>
            {
                entity.HasKey(e => e.ActionId)
                    .HasName("PK_Action");

                entity.ToTable("Action_tbl");

                entity.Property(e => e.ActionId).HasColumnName("Action_Id");

                entity.Property(e => e.ActionName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Action_name");

                entity.Property(e => e.CreatedBy)
                    .IsUnicode(false)
                    .HasColumnName("Created_By");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("date")
                    .HasColumnName("Date_Created");

                entity.Property(e => e.DateDelete)
                    .HasColumnType("date")
                    .HasColumnName("Date_Delete");

                entity.Property(e => e.DateRestored)
                    .HasColumnType("date")
                    .HasColumnName("Date_Restored");

                entity.Property(e => e.DateUpdated)
                    .HasColumnType("date")
                    .HasColumnName("Date_Updated");

                entity.Property(e => e.DeleteFlag).HasColumnName("Delete_Flag");

                entity.Property(e => e.DeletedBy)
                    .IsUnicode(false)
                    .HasColumnName("Deleted_By");

                entity.Property(e => e.ModuleId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Module_Id");

                entity.Property(e => e.RestoredBy)
                    .IsUnicode(false)
                    .HasColumnName("Restored_By");

                entity.Property(e => e.UpdatedBy)
                    .IsUnicode(false)
                    .HasColumnName("Updated_By");
            });

            modelBuilder.Entity<HBuffHerd>(entity =>
            {
                entity.ToTable("H_Buff_Herd");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address).IsUnicode(false);

                entity.Property(e => e.BBuffCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("B_Buff_Code");

                entity.Property(e => e.CreatedBy)
                    .IsUnicode(false)
                    .HasColumnName("Created_By");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("date")
                    .HasColumnName("Date_Created");

                entity.Property(e => e.DateDelete)
                    .HasColumnType("date")
                    .HasColumnName("Date_Delete");

                entity.Property(e => e.DateRestored)
                    .HasColumnType("date")
                    .HasColumnName("Date_Restored");

                entity.Property(e => e.DateUpdated)
                    .HasColumnType("date")
                    .HasColumnName("Date_Updated");

                entity.Property(e => e.DeleteFlag).HasColumnName("Delete_Flag");

                entity.Property(e => e.DeletedBy)
                    .IsUnicode(false)
                    .HasColumnName("Deleted_By");

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.FCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("F_Code");

                entity.Property(e => e.FarmAddress)
                    .IsUnicode(false)
                    .HasColumnName("Farm_Address");

                entity.Property(e => e.FarmManager)
                    .IsUnicode(false)
                    .HasColumnName("Farm_Manager");

                entity.Property(e => e.FeedCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Feed_Code");

                entity.Property(e => e.HTypeCode)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("H_Type_Code");

                entity.Property(e => e.HerdCode)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("Herd_Code");

                entity.Property(e => e.HerdName)
                    .IsUnicode(false)
                    .HasColumnName("Herd_Name");

                entity.Property(e => e.HerdSize).HasColumnName("Herd_Size");

                entity.Property(e => e.MNo)
                    .HasMaxLength(13)
                    .IsUnicode(false)
                    .HasColumnName("M_No");

                entity.Property(e => e.Owner).IsUnicode(false);

                entity.Property(e => e.RestoredBy)
                    .IsUnicode(false)
                    .HasColumnName("Restored_By");

                entity.Property(e => e.TelNo)
                    .HasMaxLength(13)
                    .IsUnicode(false)
                    .HasColumnName("Tel_No");

                entity.Property(e => e.UpdatedBy)
                    .IsUnicode(false)
                    .HasColumnName("Updated_By");
            });

            modelBuilder.Entity<HBuffaloType>(entity =>
            {
                entity.ToTable("H_Buffalo_Type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BBuffCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("B_Buff_Code");

                entity.Property(e => e.BBuffDesc)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("B_Buff_Desc");

                entity.Property(e => e.CreatedBy)
                    .IsUnicode(false)
                    .HasColumnName("Created_By");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("date")
                    .HasColumnName("Date_Created");

                entity.Property(e => e.DateDelete)
                    .HasColumnType("date")
                    .HasColumnName("Date_Delete");

                entity.Property(e => e.DateRestored)
                    .HasColumnType("date")
                    .HasColumnName("Date_Restored");

                entity.Property(e => e.DateUpdated)
                    .HasColumnType("date")
                    .HasColumnName("Date_Updated");

                entity.Property(e => e.DeleteFlag).HasColumnName("Delete_Flag");

                entity.Property(e => e.DeletedBy)
                    .IsUnicode(false)
                    .HasColumnName("Deleted_By");

                entity.Property(e => e.RestoredBy)
                    .IsUnicode(false)
                    .HasColumnName("Restored_By");

                entity.Property(e => e.UpdatedBy)
                    .IsUnicode(false)
                    .HasColumnName("Updated_By");
            });

            modelBuilder.Entity<HFarmerAffiliation>(entity =>
            {
                entity.ToTable("H_Farmer_Affiliation");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy)
                    .IsUnicode(false)
                    .HasColumnName("Created_By");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("date")
                    .HasColumnName("Date_Created");

                entity.Property(e => e.DateDelete)
                    .HasColumnType("date")
                    .HasColumnName("Date_Delete");

                entity.Property(e => e.DateRestored)
                    .HasColumnType("date")
                    .HasColumnName("Date_Restored");

                entity.Property(e => e.DateUpdated)
                    .HasColumnType("date")
                    .HasColumnName("Date_Updated");

                entity.Property(e => e.DeleteFlag).HasColumnName("Delete_Flag");

                entity.Property(e => e.DeletedBy)
                    .IsUnicode(false)
                    .HasColumnName("Deleted_By");

                entity.Property(e => e.FCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("F_Code");

                entity.Property(e => e.FDesc)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("F_Desc");

                entity.Property(e => e.RestoredBy)
                    .IsUnicode(false)
                    .HasColumnName("Restored_By");

                entity.Property(e => e.UpdatedBy)
                    .IsUnicode(false)
                    .HasColumnName("Updated_By");
            });

            modelBuilder.Entity<HFeedingSystem>(entity =>
            {
                entity.ToTable("H_Feeding_System");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy)
                    .IsUnicode(false)
                    .HasColumnName("Created_By");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("date")
                    .HasColumnName("Date_Created");

                entity.Property(e => e.DateDelete)
                    .HasColumnType("date")
                    .HasColumnName("Date_Delete");

                entity.Property(e => e.DateRestored)
                    .HasColumnType("date")
                    .HasColumnName("Date_Restored");

                entity.Property(e => e.DateUpdated)
                    .HasColumnType("date")
                    .HasColumnName("Date_Updated");

                entity.Property(e => e.DeleteFlag).HasColumnName("Delete_Flag");

                entity.Property(e => e.DeletedBy)
                    .IsUnicode(false)
                    .HasColumnName("Deleted_By");

                entity.Property(e => e.FeedCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Feed_Code");

                entity.Property(e => e.FeedDesc)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("Feed_Desc");

                entity.Property(e => e.RestoredBy)
                    .IsUnicode(false)
                    .HasColumnName("Restored_By");

                entity.Property(e => e.UpdatedBy)
                    .IsUnicode(false)
                    .HasColumnName("Updated_By");
            });

            modelBuilder.Entity<HHerdType>(entity =>
            {
                entity.ToTable("H_Herd_Type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy)
                    .IsUnicode(false)
                    .HasColumnName("Created_By");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("date")
                    .HasColumnName("Date_Created");

                entity.Property(e => e.DateDelete)
                    .HasColumnType("date")
                    .HasColumnName("Date_Delete");

                entity.Property(e => e.DateRestored)
                    .HasColumnType("date")
                    .HasColumnName("Date_Restored");

                entity.Property(e => e.DateUpdated)
                    .HasColumnType("date")
                    .HasColumnName("Date_Updated");

                entity.Property(e => e.DeleteFlag).HasColumnName("Delete_Flag");

                entity.Property(e => e.DeletedBy)
                    .IsUnicode(false)
                    .HasColumnName("Deleted_By");

                entity.Property(e => e.HTypeCode)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("H_Type_Code");

                entity.Property(e => e.HTypeDesc)
                    .HasMaxLength(17)
                    .HasColumnName("H_Type_Desc")
                    .IsFixedLength();

                entity.Property(e => e.RestoredBy)
                    .IsUnicode(false)
                    .HasColumnName("Restored_By");

                entity.Property(e => e.UpdatedBy)
                    .IsUnicode(false)
                    .HasColumnName("Updated_By");
            });

            modelBuilder.Entity<ModuleTbl>(entity =>
            {
                entity.HasKey(e => e.ModuleId)
                    .HasName("PK_Module");

                entity.ToTable("Module_tbl");

                entity.Property(e => e.ModuleId).HasColumnName("Module_Id");

                entity.Property(e => e.CreatedBy)
                    .IsUnicode(false)
                    .HasColumnName("Created_By");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("date")
                    .HasColumnName("Date_Created");

                entity.Property(e => e.DateDelete)
                    .HasColumnType("date")
                    .HasColumnName("Date_Delete");

                entity.Property(e => e.DateRestored)
                    .HasColumnType("date")
                    .HasColumnName("Date_Restored");

                entity.Property(e => e.DateUpdated)
                    .HasColumnType("date")
                    .HasColumnName("Date_Updated");

                entity.Property(e => e.DeleteFlag).HasColumnName("Delete_Flag");

                entity.Property(e => e.DeletedBy)
                    .IsUnicode(false)
                    .HasColumnName("Deleted_By");

                entity.Property(e => e.ModuleName)
                    .IsUnicode(false)
                    .HasColumnName("Module_Name");

                entity.Property(e => e.ParentModule)
                    .IsUnicode(false)
                    .HasColumnName("Parent_Module");

                entity.Property(e => e.RestoredBy)
                    .IsUnicode(false)
                    .HasColumnName("Restored_By");

                entity.Property(e => e.UpdatedBy)
                    .IsUnicode(false)
                    .HasColumnName("Updated_By");
            });

            modelBuilder.Entity<TblApiTokenModel>(entity =>
            {
                entity.ToTable("tbl_ApiTokenModel");

                entity.Property(e => e.ApiToken).IsUnicode(false);

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.Role).IsUnicode(false);
            });


            // Exclude Deleted Records
            /*modelBuilder.Entity<ABirthType>().HasQueryFilter(a => !a.DeleteFlag);
            modelBuilder.Entity<ABloodComp>().HasQueryFilter(a => !a.DeleteFlag);
            modelBuilder.Entity<ABreed>().HasQueryFilter(a => !a.DeleteFlag);
            modelBuilder.Entity<ABuffAnimal>().HasQueryFilter(a => !a.DeleteFlag);
            modelBuilder.Entity<ActionTbl>().HasQueryFilter(a => !a.DeleteFlag);
            modelBuilder.Entity<ATypeOwnership>().HasQueryFilter(a => !a.DeleteFlag);
            modelBuilder.Entity<HBuffaloType>().HasQueryFilter(a => !a.DeleteFlag);
            modelBuilder.Entity<HBuffHerd>().HasQueryFilter(a => !a.DeleteFlag);
            modelBuilder.Entity<HFarmerAffiliation>().HasQueryFilter(a => !a.DeleteFlag);*/
            modelBuilder.Entity<HFeedingSystem>().HasQueryFilter(a => !a.DeleteFlag);
            /*modelBuilder.Entity<HHerdType>().HasQueryFilter(a => !a.DeleteFlag);
            modelBuilder.Entity<ModuleTbl>().HasQueryFilter(a => !a.DeleteFlag);*/

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
