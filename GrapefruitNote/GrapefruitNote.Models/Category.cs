namespace GrapefruitNote.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public class Category
    {
        private ICollection<Note> notes;

        public Category()
        {
            this.notes = new HashSet<Note>();
        }

        [Key]
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<Note> Notes
        {
            get
            {
                return this.notes;
            }
            set
            {
                this.notes = value;
            }
        }
    }
}