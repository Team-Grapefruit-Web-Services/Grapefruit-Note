namespace GrapefruitNote.Data
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using GrapefruitNote.Models;
    using GrapefruitNote.Data.Migrations;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Data.Entity.ModelConfiguration.Conventions;


    public class GrapefruitNoteDbContext : DbContext, IGrapefruitNoteDbContext
    {
        public GrapefruitNoteDbContext()
            : base("GrapefruitNote")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<GrapefruitNoteDbContext, Configuration>());
        }

        public static GrapefruitNoteDbContext Create()
        {
            return new GrapefruitNoteDbContext();
        }

        public IDbSet<User> Users { get; set; }

        public IDbSet<Note> Notes { get; set; }

        public IDbSet<Category> Categories { get; set; }

        public new void SaveChanges()
        {
            base.SaveChanges();
        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Flight>()
            //            .HasRequired(f => f.DepartureAirport)
            //            .WithMany(a => a.DepartureFlights)
            //            .HasForeignKey(f => f.DepartureAirportId)
            //            .WillCascadeOnDelete(false);

            //modelBuilder.Entity<User>()
            //    .HasRequired(u => u.Notes.Single())
            //    .WithMany(x => x.Categories)
            //    .HasForeignKey(c => c.)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();   

        }
    }
}