namespace GrapefruitNote.Services.Controllers
{
    using GrapefruitNote.Data;
    using System.Web.Http;

    public abstract class BaseApiController : ApiController
    {
        protected IGrapefruitNoteData data;

        protected BaseApiController(IGrapefruitNoteData data)
        {
            this.data = data;
        }
    }
}