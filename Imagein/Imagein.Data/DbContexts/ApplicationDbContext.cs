using Imagein.Data.Helpers;
using Imagein.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imagein.Data.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<FileEntity> Files { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Always get dates in UTC
            optionsBuilder.ReplaceService<IEntityMaterializerSource, CustomEntityMaterializeSource>();

            base.OnConfiguring(optionsBuilder);
        }

        public override int SaveChanges()
        {
            // Always save dates in UTC
            DateTimeUtcHelper.SetDatesToUtc(this.ChangeTracker.Entries());

            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);


            const string PG_TIMESTAMP_TYPE_NAME = "timestamp";
            const string PG_TIMESTAMP_UTC_GENERATE_COMMAND = "CAST(NOW() at time zone 'utc' AS timestamp)";

            // Take length considering storing: 
            // GUID 38 chars (not use 68 chars Hexadecimal format), 
            // UUID 36 chars, but can be 39 in some of alternative forms 
            // Ulid 26 chars
            const string ID_FILED_TYPE = "character varying(39)";


            // File
            builder.Entity<FileEntity>()
                .HasKey(f => f.Id);

            builder.Entity<FileEntity>()
                .Property(f => f.Id)
                .HasColumnType(ID_FILED_TYPE)
                .IsRequired();

            builder.Entity<FileEntity>()
                .Property(f => f.CreatedOnUtc)
                .HasColumnType(PG_TIMESTAMP_TYPE_NAME)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql(PG_TIMESTAMP_UTC_GENERATE_COMMAND);

            builder.Entity<FileEntity>()
                .Property(f => f.UpdatedOnUtc)
                .HasColumnType(PG_TIMESTAMP_TYPE_NAME)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql(PG_TIMESTAMP_UTC_GENERATE_COMMAND);
        }
    }
}
