namespace GrapefruitNote.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public class Note
    {
        private ICollection<Category> categories;
        
        public Note()
        {
            this.categories = new HashSet<Category>();
        }

        [Key]
        public int NoteId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Text { get; set; }

        public DateTime EntryDate { get; set; }

        public DateTime DueDate { get; set; }

        public Priority Priority { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        public virtual User User { get; set; }

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