using System;
using System.Collections.Generic;
using System.Linq;
using HiloCore;
using Microsoft.EntityFrameworkCore;

namespace HiloWithEF {

    class Program {

        static PgContext database;

        static void Main(string[] args) {
            database = new PgContext();
            var entities = CreateSampleEntities(50);
            SaveEntities(entities);
            QueryEntities();
            Console.ReadLine();
            database.Dispose();
        }

        private static void QueryEntities() {
            var query = from entity in database.Entries
                group entity by entity.Text into g
                select new {
                    Key = g.Key,
                    Count = g.Count(),
                    MinId = g.Min(e => e.Id),
                    MaxId = g.Max(e => e.Id)
                };
            var result = query.ToList();
            Console.WriteLine(result.Count);
        }

        private static void SaveEntities(IList<SampleEntity> entities) {
            database.Entries.AddRange(entities);
            database.SaveChanges();
            // var tx = database.Database.BeginTransaction();
            // try {
            //     database.Entries.AddRange(entities);
            //     database.SaveChanges();
            //     tx.Commit();
            // }
            // catch (Exception ex) {
            //     tx.Rollback();
            //     Console.WriteLine(ex);
            // }
        }

        private static IList<SampleEntity> CreateSampleEntities(int count) {
            var result = new List<SampleEntity>(count);
            for (var i = 0; i < count; i++) {
                result.Add(new SampleEntity {
                    SubId = i + 1,
                    Title = $"title for {i + 1}",
                    Text = "EntityFrameworkCore"
                });
            }
            return result;
        }
    }

    public class PgContext : DbContext {

        public DbSet<SampleEntity> Entries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseNpgsql(
                "server=127.0.0.1;database=test_db;user id=postgres;password=1a2b3c4D;",
                b => b.MaxBatchSize(50)
            );
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            // base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("public");
            modelBuilder.HasSequence(
                "ef_core_hilo_sequence",
                b => b.IncrementsBy(100)
            );
            modelBuilder.Entity<SampleEntity>()
                .ToTable("entries");
            modelBuilder.Entity<SampleEntity>()
                .Property("Id")
                .ForNpgsqlUseSequenceHiLo("ef_core_hilo_sequence")
                .HasColumnName("id")
                .HasColumnType("integer");
            modelBuilder.Entity<SampleEntity>()
                .Property("SubId")
                .HasColumnName("sub_id")
                .HasColumnType("integer");
            modelBuilder.Entity<SampleEntity>()
                .Property("Title")
                .HasColumnName("title")
                .HasColumnType("character varying")
                .HasMaxLength(64);
            modelBuilder.Entity<SampleEntity>()
                .Property("Text")
                .HasColumnName("text")
                .HasColumnType("character varying")
                .HasMaxLength(1024);
            modelBuilder.Entity<SampleEntity>().HasKey("Id");
        }
    }
}
