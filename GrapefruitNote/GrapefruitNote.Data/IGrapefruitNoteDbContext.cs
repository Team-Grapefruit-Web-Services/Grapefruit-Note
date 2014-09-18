namespace GrapefruitNote.Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;

    using GrapefruitNote.Models;

    public interface IGrapefruitNoteDbContext 
    {
        IDbSet<User> Users { get; set; }

        IDbSet<Note> Notes { get; set; }

        IDbSet<Category> Categories { get; set; }

        void SaveChanges();

        IDbSet<TEntity> Set<TEntity>() where TEntity : class;

        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    }
}
