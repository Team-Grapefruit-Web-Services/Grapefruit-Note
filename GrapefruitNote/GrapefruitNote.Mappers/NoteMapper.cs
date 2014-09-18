namespace GrapefruitNote.Mappers
{
    using GrapefruitNote.DataTransferObjects;
    using GrapefruitNote.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;

    public class NoteMapper
    {
        public static Expression<Func<Note, NoteModel>> ToNoteModel
        {
            get
            {
                return note => new NoteModel
                {
                    Title = note.Title,
                    Text = note.Text,
                    EntryDate = note.EntryDate,
                    DueDate = note.DueDate,
                    Priority = (int)note.Priority,
                    Categories = note.Categories
                        .AsQueryable()
                        .Select(CategoryMapper.ToCategoryModel)
                        .ToList()
                };
            }
        }

        public static Expression<Func<NoteModel, Note>> ToNoteEntity
        {
            get
            {
                return noteModel => new Note
                {
                    Title = noteModel.Title,
                    Text = noteModel.Text,
                    EntryDate = noteModel.EntryDate,
                    DueDate = noteModel.DueDate,
                    Priority = (Priority)noteModel.Priority,
                    Categories = noteModel.Categories
                        .AsQueryable()
                        .Select(CategoryMapper.ToCategoryEntity)
                        .ToList()
                };
            }
        }
    }
}
