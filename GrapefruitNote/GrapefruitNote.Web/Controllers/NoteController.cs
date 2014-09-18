namespace GrapefruitNote.Web.Controllers
{
    using GrapefruitNote.Data;
    using GrapefruitNote.Mappers;
    using GrapefruitNote.DataTransferObjects;
    using GrapefruitNote.Web.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using GrapefruitNote.Models;

    [Authorize]
    public class NoteController : BaseApiController
    {
        private readonly IUserIdProvider userIdProvider;

        public NoteController()
            :this(new GrapefruitNoteData(), new AspNetUserIdProvider())
        {
        }

        public NoteController(IGrapefruitNoteData data, IUserIdProvider userIdProvider)
            :base(data)
        {
            this.userIdProvider = userIdProvider;
        }

        [HttpGet]
        public IHttpActionResult All()
        {
            var currentUserId = this.userIdProvider.GetUserId();

            var allNotes = this.data.Notes.All()
                .Where(n => n.UserId == currentUserId)
                .Select(NoteMapper.ToNoteModel);

            return Ok(allNotes);
        }

        [HttpPost]
        public IHttpActionResult Create(NoteModel noteModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var currentUserId = this.userIdProvider.GetUserId();

            var compiledModelToEntityExpression = NoteMapper.ToNoteEntity.Compile();
            var newNote = compiledModelToEntityExpression(noteModel);
            newNote.UserId = currentUserId;

            this.data.Notes.Add(newNote);
            this.data.SaveChanges();

            noteModel.NoteId = newNote.NoteId;
            return Ok(noteModel);
        }

        [HttpPut]
        public IHttpActionResult Update(int id, NoteModel noteModel)
        {
            var existingNote = this.GetNoteById(id);

            if (existingNote == null)
            {
                return this.BadRequest("Note with this id does not exist.");
            }

            var currentUserId = this.userIdProvider.GetUserId();

            if (currentUserId != existingNote.UserId)
            {
                return this.BadRequest("Unauthorized access.");
            }

            existingNote.Title = noteModel.Title;
            existingNote.Text = noteModel.Text;
            existingNote.Priority = (Priority)noteModel.Priority;
            existingNote.EntryDate = noteModel.EntryDate;
            existingNote.DueDate = noteModel.DueDate;
            existingNote.Categories = noteModel.Categories.AsQueryable().Select(CategoryMapper.ToCategoryEntity).ToList();

            this.data.SaveChanges();
            return Ok(existingNote);
        }

        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var existingNote = this.GetNoteById(id);

            if (existingNote == null)
            {
                return this.BadRequest("Note with this id does not exist.");
            }

            var currentUserId = this.userIdProvider.GetUserId();

            if (currentUserId != existingNote.UserId)
            {
                return this.BadRequest("Unauthorized access.");
            }

            this.data.Notes.Delete(existingNote);
            this.data.SaveChanges();

            return this.Ok();
        }

        [HttpPut]
        public IHttpActionResult AddCategory(int noteId, int categoryId)
        {
            var existingNote = this.GetNoteById(noteId);

            if (existingNote == null)
            {
                return this.BadRequest("Note with this id does not exist.");
            }

            var currentUserId = this.userIdProvider.GetUserId();

            if (currentUserId != existingNote.UserId)
            {
                return this.BadRequest("You have no permissions to edit this note.");
            }

            var category = this.data.Categories.All().FirstOrDefault(c => c.CategoryId == categoryId);

            if (category == null)
            {
                return this.BadRequest("Invalid category id");
            }

            if (category.UserId != currentUserId)
            {
                return this.BadRequest("Unauthorized access.");
            }

            existingNote.Categories.Add(category);
            this.data.SaveChanges();

            return Ok();
        }

        public IHttpActionResult RemoveCategory(int noteId, int categoryId)
        {
            var existingNote = this.GetNoteById(noteId);

            if (existingNote == null)
            {
                return this.BadRequest("Note with this id does not exist.");
            }

            var currentUserId = this.userIdProvider.GetUserId();

            if (currentUserId != existingNote.UserId)
            {
                return this.BadRequest("You have no permissions to edit this note.");
            }

            var category = this.data.Categories.All().FirstOrDefault(c => c.CategoryId == categoryId);

            if (category == null)
            {
                return this.BadRequest("Invalid category id");
            }

            if (category.UserId != currentUserId)
            {
                return this.BadRequest("Unauthorized access.");
            }

            existingNote.Categories.Remove(category);
            this.data.SaveChanges();

            return Ok();

        }

        private Note GetNoteById(int id)
        {
            return this.data.Notes.All().FirstOrDefault(n => n.NoteId == id);
        }
    }
}