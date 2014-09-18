namespace GrapefruitNote.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class Note
    {
        private ICollection<Category> categories;

        public Note()
        {
            this.categories = new HashSet<Category>();
        }

        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Text { get; set; }

        public DateTime EntryTime { get; set; }

        public DateTime DueDate { get; set; }

        public Priority Priority { get; set; }

        public virtual ICollection<Category> Categories
        {
            get
            {
                return this.categories;
            }
            set
            {
                this.categories = value;
            }
        }
    }
}