namespace GrapefruitNote.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class User
    {
        private ICollection<Note> notes;

        public User()
        {
            this.notes = new HashSet<Note>();
        }

        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(10)]
        public string Username { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string ProfilePicture { get; set; }

        public virtual ICollection<Note> Notes
        {
            get { return this.notes; }
            set { this.notes = value; }
        }
    }
}