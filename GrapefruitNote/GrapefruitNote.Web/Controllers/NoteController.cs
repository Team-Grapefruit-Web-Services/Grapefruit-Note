using GrapefruitNote.Data;
using GrapefruitNote.Web.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace GrapefruitNote.Web.Controllers
{
    [Authorize]
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