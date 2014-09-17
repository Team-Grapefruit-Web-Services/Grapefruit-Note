namespace GrapefruitNote.Data
{
    using System;
    using System.Linq;

    using GrapefruitNote.Models;
    using GrapefruitNote.Data.Repositories;
    using System.Collections.Generic;

    public class GrapefruitNoteData : IGrapefruitNoteData
    {
        private readonly IGrapefruitNoteDbContext context;
        private readonly IDictionary<Type, object> repositories = new Dictionary<Type, object>();

        public GrapefruitNoteData()
            : this(new GrapefruitNoteDbContext())
        {
        }

        public GrapefruitNoteData(IGrapefruitNoteDbContext context)
        {
            this.context = context;
        }

        public IRepository<User> Users
        {
            get
            {
                return this.GetRepository<User>();
            }
        }

        public IRepository<Note> Notes
        {
            get
            {
                return this.GetRepository<Note>();
            }
        }

        public IRepository<Category> Categories
        {
            get
            {
                return this.GetRepository<Category>();
            }
        }

        public void SaveChanges()
        {
            this.context.SaveChanges();
        }

        private IRepository<T> GetRepository<T>() where T : class
        {
            var typeOfModel = typeof(T);

            if (!this.repositories.ContainsKey(typeOfModel))
            {
                var type = typeof(Repository<T>);
                this.repositories.Add(typeOfModel, Activator.CreateInstance(type, this.context));
            }

            return (IRepository<T>)this.repositories[typeOfModel];
        }
    }
}
