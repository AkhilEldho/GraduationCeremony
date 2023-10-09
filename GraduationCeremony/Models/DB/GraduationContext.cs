using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GraduationCeremony.Models.DB
{
    public partial class GraduationContext : DbContext
    {
        public GraduationContext()
        {
        }

        public GraduationContext(DbContextOptions<GraduationContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Graduation> Graduations { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=citizen.manukautech.info,6302;Database=S232_Project01_Test;UID=S232_Project01;PWD=fBit$20843;encrypt=true;trustservercertificate=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Graduation>(entity =>
            {
                entity.HasKey(e => e.PersonCode)
                    .HasName("PK__Graduati__F2E6F31CC7B28506");

                entity.ToTable("Graduation");

                entity.Property(e => e.PersonCode)
                    .ValueGeneratedNever()
                    .HasColumnName("PERSON_CODE");

                entity.Property(e => e.AcademicDressRequirements1)
                    .HasMaxLength(50)
                    .HasColumnName("ACADEMIC_DRESS_REQUIREMENTS_1");

                entity.Property(e => e.AcademicDressRequirements2)
                    .HasMaxLength(50)
                    .HasColumnName("ACADEMIC_DRESS_REQUIREMENTS_2");

                entity.Property(e => e.AddressLine1)
                    .HasMaxLength(1)
                    .HasColumnName("ADDRESS_LINE_1");

                entity.Property(e => e.AddressLine2)
                    .HasMaxLength(50)
                    .HasColumnName("ADDRESS_LINE_2");

                entity.Property(e => e.AddressLine3)
                    .HasMaxLength(1)
                    .HasColumnName("ADDRESS_LINE_3");

                entity.Property(e => e.AddressLine4)
                    .HasMaxLength(1)
                    .HasColumnName("ADDRESS_LINE_4");

                entity.Property(e => e.AwardCode)
                    .HasMaxLength(50)
                    .HasColumnName("AWARD_CODE");

                entity.Property(e => e.AwardDescription)
                    .HasMaxLength(100)
                    .HasColumnName("AWARD_DESCRIPTION");

                entity.Property(e => e.Awarded)
                    .HasColumnType("date")
                    .HasColumnName("AWARDED");

                entity.Property(e => e.BadDebtStatus).HasColumnName("BAD_DEBT_STATUS");

                entity.Property(e => e.Campus)
                    .HasMaxLength(1)
                    .HasColumnName("CAMPUS");

                entity.Property(e => e.CollegeEmail)
                    .HasMaxLength(1)
                    .HasColumnName("COLLEGE_EMAIL");

                entity.Property(e => e.Comments)
                    .HasMaxLength(50)
                    .HasColumnName("COMMENTS");

                entity.Property(e => e.Completion)
                    .HasColumnType("date")
                    .HasColumnName("COMPLETION");

                entity.Property(e => e.Credits).HasColumnName("CREDITS");

                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("date")
                    .HasColumnName("DATE_OF_BIRTH");

                entity.Property(e => e.DateRecordAddedToMasterList)
                    .HasColumnType("date")
                    .HasColumnName("DATE_RECORD_ADDED_TO_MASTER_LIST");

                entity.Property(e => e.Ethnicity1)
                    .HasMaxLength(50)
                    .HasColumnName("ETHNICITY1");

                entity.Property(e => e.Ethnicity2)
                    .HasMaxLength(50)
                    .HasColumnName("ETHNICITY2");

                entity.Property(e => e.Ethnicity3)
                    .HasMaxLength(50)
                    .HasColumnName("ETHNICITY3");

                entity.Property(e => e.Forenames)
                    .HasMaxLength(50)
                    .HasColumnName("FORENAMES");

                entity.Property(e => e.IncludeInSdr).HasColumnName("INCLUDE_IN_SDR");

                entity.Property(e => e.Iwi1)
                    .HasMaxLength(100)
                    .HasColumnName("IWI_1");

                entity.Property(e => e.Iwi2).HasColumnName("IWI_2");

                entity.Property(e => e.Iwi3).HasColumnName("IWI_3");

                entity.Property(e => e.Level)
                    .HasMaxLength(50)
                    .HasColumnName("LEVEL");

                entity.Property(e => e.Major1)
                    .HasMaxLength(50)
                    .HasColumnName("MAJOR_1");

                entity.Property(e => e.Major2)
                    .HasMaxLength(50)
                    .HasColumnName("MAJOR_2");

                entity.Property(e => e.Mobile).HasColumnName("MOBILE");

                entity.Property(e => e.Nsn).HasColumnName("NSN");

                entity.Property(e => e.PersonalEmail)
                    .HasMaxLength(1)
                    .HasColumnName("PERSONAL_EMAIL");

                entity.Property(e => e.PostCode)
                    .HasMaxLength(1)
                    .HasColumnName("POST_CODE");

                entity.Property(e => e.QualificationCode)
                    .HasMaxLength(50)
                    .HasColumnName("QUALIFICATION_CODE");

                entity.Property(e => e.School)
                    .HasMaxLength(1)
                    .HasColumnName("SCHOOL");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("STATUS");

                entity.Property(e => e.Surname)
                    .HasMaxLength(50)
                    .HasColumnName("SURNAME");

                entity.Property(e => e.Telephone)
                    .HasMaxLength(1)
                    .HasColumnName("TELEPHONE");

                entity.Property(e => e.Town)
                    .HasMaxLength(1)
                    .HasColumnName("TOWN");

                entity.Property(e => e.YearArchieved)
                    .HasColumnType("date")
                    .HasColumnName("YEAR_ARCHIEVED");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
