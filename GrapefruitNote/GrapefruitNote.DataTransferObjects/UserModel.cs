namespace GrapefruitNote.DataTransferObjects
{
    using System.Collections.Generic;

    public class UserModel
    {
        public UserModel()
        {
            this.Notes = new HashSet<NoteModel>();
            this.Categories = new HashSet<CategoryModel>();
        }   

        public string ProfilePictureUrl { get; set; }

        public virtual ICollection<NoteModel> Notes { get; set; }

        public virtual ICollection<CategoryModel> Categories { get; set; }
    }
}
