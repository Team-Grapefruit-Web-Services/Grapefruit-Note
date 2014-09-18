namespace GrapefruitNote.Web.Controllers
{
    using GrapefruitNote.Data;
    using System.Web.Http;

    [Authorize]
    public abstract class BaseApiController : ApiController
    {
        protected IGrapefruitNoteData data;

        protected BaseApiController(IGrapefruitNoteData data)
        {
            this.data = data;
        }
    }
}