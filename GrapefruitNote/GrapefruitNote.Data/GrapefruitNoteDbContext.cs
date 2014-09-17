namespace GrapefruitNote.Data
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    using GrapefruitNote.Models;
    using GrapefruitNote.Data.Migrations;

    public class GrapefruitNoteDbContext: DbContext, IGrapefruitNoteDbContext
    {
        public GrapefruitNoteDbContext()
            : base("GrapefruitNote")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<GrapefruitNoteDbContext, Configuration>());
        }

        public virtual IDbSet<User> Users { get; set; }

        public virtual IDbSet<Note> Notes { get; set; }

        public virtual IDbSet<Category> Categories { get; set; }

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