namespace GrapefruitNote.Models
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class User
    {
        private ICollection<Note> notes;
        private ICollection<Category> categories;

        public User()
        {
            this.Notes = new HashSet<Note>();
            this.Categories = new HashSet<Category>();
        }

        [Key]
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string AuthCode { get; set; }

        public string SessionKey { get; set; }

        public string ProfilePictureUrl { get; set; }

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
