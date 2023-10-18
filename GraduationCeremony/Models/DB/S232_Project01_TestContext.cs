using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GraduationCeremony.Models.DB
{
    public partial class S232_Project01_TestContext : DbContext
    {
        public S232_Project01_TestContext()
        {
        }

        public S232_Project01_TestContext(DbContextOptions<S232_Project01_TestContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AspNetRole> AspNetRoles { get; set; } = null!;
        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; } = null!;
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; } = null!;
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; } = null!;
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; } = null!;
        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; } = null!;
        public virtual DbSet<Award> Awards { get; set; } = null!;
        public virtual DbSet<CheckIn> CheckIns { get; set; } = null!;
        public virtual DbSet<Graduand> Graduands { get; set; } = null!;
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
            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetRoleClaim>(entity =>
            {
                entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email)
                    .HasMaxLength(256)
                    .HasDefaultValueSql("(N'')");

                entity.Property(e => e.FirstName).HasDefaultValueSql("(N'')");

                entity.Property(e => e.LastName).HasDefaultValueSql("(N'')");

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.Password).HasDefaultValueSql("(N'')");

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.HasMany(d => d.Roles)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "AspNetUserRole",
                        l => l.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                        r => r.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId");

                            j.ToTable("AspNetUserRoles");

                            j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                        });
            });

            modelBuilder.Entity<AspNetUserClaim>(entity =>
            {
                entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<Award>(entity =>
            {
                entity.ToTable("Award");

                entity.Property(e => e.AwardId).HasColumnName("AwardID");

                entity.Property(e => e.AwardCode)
                    .HasMaxLength(50)
                    .HasColumnName("Award_code");

                entity.Property(e => e.AwardDescription)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("Award_description");

                entity.Property(e => e.Level).HasMaxLength(50);

                entity.Property(e => e.QualificationCode)
                    .HasMaxLength(50)
                    .HasColumnName("Qualification_code");

                entity.Property(e => e.School)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CheckIn>(entity =>
            {
                entity.HasKey(e => e.PersonCode)
                    .HasName("PK__CheckIn__F2E6F31C55197C43");

                entity.ToTable("CheckIn");

                entity.Property(e => e.PersonCode)
                    .ValueGeneratedNever()
                    .HasColumnName("PERSON_CODE");

                entity.Property(e => e.AwardCode)
                    .HasMaxLength(50)
                    .HasColumnName("AWARD_CODE");

                entity.Property(e => e.AwardDescription)
                    .HasMaxLength(100)
                    .HasColumnName("AWARD_DESCRIPTION");

                entity.Property(e => e.CollegeEmail)
                    .HasMaxLength(1)
                    .HasColumnName("COLLEGE_EMAIL");

                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("date")
                    .HasColumnName("DATE_OF_BIRTH");

                entity.Property(e => e.Forenames)
                    .HasMaxLength(50)
                    .HasColumnName("FORENAMES");

                entity.Property(e => e.Level)
                    .HasMaxLength(50)
                    .HasColumnName("LEVEL");

                entity.Property(e => e.Mobile).HasColumnName("MOBILE");

                entity.Property(e => e.Nsn).HasColumnName("NSN");

                entity.Property(e => e.QualificationCode)
                    .HasMaxLength(50)
                    .HasColumnName("QUALIFICATION_CODE");

                entity.Property(e => e.Surname)
                    .HasMaxLength(50)
                    .HasColumnName("SURNAME");
            });

            modelBuilder.Entity<Graduand>(entity =>
            {
                entity.HasKey(e => e.PersonCode);

                entity.ToTable("Graduand");

                entity.Property(e => e.PersonCode)
                    .ValueGeneratedNever()
                    .HasColumnName("Person_code");

                entity.Property(e => e.AddressLine1)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("Address_line_1");

                entity.Property(e => e.AddressLine2)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("Address_line_2");

                entity.Property(e => e.AddressLine3)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("Address_line_3");

                entity.Property(e => e.AddressLine4)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("Address_line_4");

                entity.Property(e => e.BadDebtStatus)
                    .HasMaxLength(50)
                    .HasColumnName("Bad_debt_status");

                entity.Property(e => e.Campus)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CollegeEmail)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("College_email");

                entity.Property(e => e.Comments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("date")
                    .HasColumnName("Date_of_Birth");

                entity.Property(e => e.DateRecordAddedToMasterList)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("Date_record_added_to_master_list");

                entity.Property(e => e.Ethnicity1)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("Ethnicity_1");

                entity.Property(e => e.Ethnicity2)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("Ethnicity_2");

                entity.Property(e => e.Ethnicity3)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("Ethnicity_3");

                entity.Property(e => e.Forenames).HasMaxLength(50);

                entity.Property(e => e.Iwi1)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("Iwi_1");

                entity.Property(e => e.Iwi2)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("Iwi_2");

                entity.Property(e => e.Iwi3)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("Iwi_3");

                entity.Property(e => e.Mobile)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nsn).HasColumnName("NSN");

                entity.Property(e => e.PersonalEmail)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Personal_email");

                entity.Property(e => e.Postcode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.School)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.Surname).HasMaxLength(50);

                entity.Property(e => e.Telephone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Town)
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Graduation>(entity =>
            {
                entity.HasKey(e => e.PersonCode)
                    .HasName("PK__Graduati__F2E6F31CE8A53C09");

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
                    .HasMaxLength(50)
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
