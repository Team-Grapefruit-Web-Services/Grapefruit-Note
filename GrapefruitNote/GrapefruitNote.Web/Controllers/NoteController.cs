using GrapefruitNote.Data;
using GrapefruitNote.Web.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GrapefruitNote.Web.Controllers
{
    public class NoteController : BaseApiController
    {
        private readonly IUserIdProvider userIdProvider;

        public NoteController(IGrapefruitNoteData data, IUserIdProvider userIdProvider)
            :base(data)
        {
            this.userIdProvider = userIdProvider;
        }
    }
}