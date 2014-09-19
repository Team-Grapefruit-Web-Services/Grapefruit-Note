namespace GrapefruitNote.Services.Controllers
{
    using GrapefruitNote.Data;
    using GrapefruitNote.Mappers;
    using GrapefruitNote.DataTransferObjects;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using GrapefruitNote.Models;

    public class NoteController : BaseApiController
    {
        private readonly Func<NoteModel, Note> toNoteEntity = NoteMapper.ToNoteEntity.Compile();
        
        public NoteController()
            :this(new GrapefruitNoteData())
        {
        }

        public NoteController(IGrapefruitNoteData data)
            :base(data)
        {
        }

        [HttpGet, ActionName("notes")]
        public IHttpActionResult All(string sessionKey)
        {
            var currentUser = this.GetUserBySessionKey(sessionKey);

            if (currentUser == null)
            {
                return BadRequest("Invalid user");
            }

            var allNotes = this.data.Notes.All()
                .Where(n => n.UserId == currentUser.UserId)
                .Select(NoteMapper.ToNoteModel);

            return Ok(allNotes);
        }

        [HttpPost, ActionName("create")]
        public IHttpActionResult Create(string sessionKey, NoteModel noteModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var currentUser = this.GetUserBySessionKey(sessionKey);

            if (currentUser == null)
            {
                return BadRequest("Invalid user");
            }

            var newNote = toNoteEntity(noteModel);
            newNote.UserId = currentUser.UserId;

            this.data.Notes.Add(newNote);
            this.data.SaveChanges();

            noteModel.NoteId = newNote.NoteId;
            return Ok(noteModel);
        }

        [HttpPut, ActionName("update")]
        public IHttpActionResult Update(string sessionKey, int id, NoteModel noteModel)
        {
            var currentUser = this.GetUserBySessionKey(sessionKey);

            if (currentUser == null)
            {
                return BadRequest("Invalid user");
            }

            var existingNote = currentUser.Notes.FirstOrDefault(n => n.NoteId == id);

            if (existingNote == null)
            {
                return this.BadRequest("Note with this id does not exist.");
            }

            var updated = toNoteEntity(noteModel);
            existingNote = updated;
            this.data.SaveChanges();

            return Ok(existingNote);
        }

        [HttpGet, ActionName("getByPriority")]
        public IHttpActionResult GetByPriority(string sessionKey, int priority)
        {
            var currentUser = this.GetUserBySessionKey(sessionKey);

            if (currentUser == null)
            {
                return BadRequest("Invalid user");
            }

            var notes = this.data.Notes.All()
                .Where(n => 
                    n.UserId == currentUser.UserId
                    && (int)n.Priority == priority)
                .Select(NoteMapper.ToNoteModel);

            return Ok(notes);
        }

        [HttpDelete, ActionName("delete")]
        public IHttpActionResult Delete(string sessionKey, int id)
        {
            var currentUser = this.GetUserBySessionKey(sessionKey);

            if (currentUser == null)
            {
                return BadRequest("Unauthorized access.");
            }

            var existingNote = currentUser.Notes.FirstOrDefault(n => n.NoteId == id);

            if (existingNote == null)
            {
                return this.BadRequest("Note with this id does not exist.");
            }

            this.data.Notes.Delete(existingNote);
            this.data.SaveChanges();

            return this.Ok();
        }

        [HttpPost, ActionName("addCategory")]
        public IHttpActionResult AddCategory(string sessionKey, int noteId, int categoryId)
        {
            var existingNote = this.GetNoteById(noteId);

            if (existingNote == null)
            {
                return this.BadRequest("Note with this id does not exist.");
            }

            var currentUser = this.GetUserBySessionKey(sessionKey);

            if (currentUser == null)
            {
                return BadRequest("Unauthorized access.");
            }

            var category = this.data.Categories.All().FirstOrDefault(c => c.CategoryId == categoryId);

            if (category == null)
            {
                return this.BadRequest("Invalid category id");
            }

            if (category.UserId != currentUser.UserId)
            {
                return this.BadRequest("Unauthorized access.");
            }

            existingNote.Categories.Add(category);
            this.data.SaveChanges();

            return Ok();
        }

        [HttpPost, ActionName("removeCategory")]
        public IHttpActionResult RemoveCategory(string sessionKey, int noteId, int categoryId)
        {
            var existingNote = this.GetNoteById(noteId);

            if (existingNote == null)
            {
                return this.BadRequest("Note with this id does not exist.");
            }

            var currentUser = this.GetUserBySessionKey(sessionKey);

            if (currentUser == null)
            {
                return BadRequest("Invalid user");
            }

            var category = this.data.Categories.All().FirstOrDefault(c => c.CategoryId == categoryId);

            if (category == null)
            {
                return this.BadRequest("Invalid category id");
            }

            if (category.UserId != currentUser.UserId)
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

        private User GetUserBySessionKey(string sessionKey)
        {
            return this.data.Users.All().FirstOrDefault(u => u.SessionKey == sessionKey);
        }
    }
}