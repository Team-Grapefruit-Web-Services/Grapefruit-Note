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


    public class GrapefruitNoteDbContext : IdentityDbContext<User>, IGrapefruitNoteDbContext
    {
        public GrapefruitNoteDbContext()
            : base("GrapefruitNote", throwIfV1Schema: false)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<GrapefruitNoteDbContext, Configuration>());
        }

        public static GrapefruitNoteDbContext Create()
        {
            return new GrapefruitNoteDbContext();
        }

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
    }
}