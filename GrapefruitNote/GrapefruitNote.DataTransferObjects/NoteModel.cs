namespace GrapefruitNote.DataTransferObjects
{
    using System;
    using System.Collections.Generic;

    public class NoteModel
    {
        public NoteModel()
        {
            this.Categories = new List<CategoryModel>();
        }

        public string Title { get; set; }

        public string Text { get; set; }

        public DateTime EntryDate { get; set; }

        public DateTime DueDate { get; set; }

        public int Priority { get; set; }

        public ICollection<CategoryModel> Categories { get; set; }
    }
}
