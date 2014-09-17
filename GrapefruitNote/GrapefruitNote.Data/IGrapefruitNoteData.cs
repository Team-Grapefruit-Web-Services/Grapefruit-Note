namespace GrapefruitNote.Data
{
    using System;
    using System.Linq;
    using GrapefruitNote.Data.Repositories;
    using GrapefruitNote.Models;

    public interface IGrapefruitNoteData
    {
        IRepository<User> Users { get; }

        IRepository<Note> Notes { get; }

        IRepository<Category> Categories { get; }

        void SaveChanges();
    }
}
