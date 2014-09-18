using GrapefruitNote.Data;
using GrapefruitNote.Web.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Threading;
using System.Threading.Tasks;

using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using System.Web;

namespace GrapefruitNote.Web.Controllers
{
    [Authorize]
    public class CloudStorageController : BaseApiController
    {
        private const string GoogleDriveClientId = "735079433623-28dbigh1qehsp5k4tmpph2i3s40gr41t.apps.googleusercontent.com";
        private const string GoogleDriveSecret = "2XzoOtuoNhV_AQogZnInr_ol";
        private const  string GoogleDriveClientName = "user";
        private const string GoogleDriveApplicationName = "TestingGoogleDriveAPI";
        private const string FileDefaultTitle = "GrapefruitNote";
        private const string FileDefaultDescription = "A test document";

        private readonly IUserIdProvider userIdProvider;

        public CloudStorageController()
            :this(new GrapefruitNoteData(), new AspNetUserIdProvider())
        {
        }

        public CloudStorageController(IGrapefruitNoteData data, IUserIdProvider userIdProvider)
            :base(data)
        {
            this.userIdProvider = userIdProvider;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> UploadPicture()
        {
            if (!this.ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            //create credentials
            UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = GoogleDriveClientId,
                    ClientSecret = GoogleDriveSecret
                },
                new[] { DriveService.Scope.Drive },
                GoogleDriveClientName,
                CancellationToken.None).Result;

            // Create the service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = GoogleDriveApplicationName,
            });

            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = new MultipartMemoryStreamProvider();

            try
            {
                await TaskEx.Run(async () => await Request.Content.ReadAsMultipartAsync(provider));

                foreach (var item in provider.Contents)
                {
                    File body = new File();
                    body.Title = FileDefaultTitle;
                    body.Description = FileDefaultDescription;
                    body.MimeType = item.Headers.ContentType.MediaType;

                    byte[] byteArray = item.ReadAsByteArrayAsync().Result;

                    System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);

                    FilesResource.InsertMediaUpload request = service.Files.Insert(body, stream, item.Headers.ContentType.MediaType);
                    request.Upload();

                    File file = request.ResponseBody;
                    string newPictureUrl = file.WebContentLink.Substring(0, file.WebContentLink.Length - 16);

                    //Change profile picture in DB.
                    this.SetUserProfilePicture(newPictureUrl);

                    //Return the new profile picture url to the client.
                    return Request.CreateResponse(HttpStatusCode.Created, newPictureUrl);
                }
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest, "No image sent");
        }

        private void SetUserProfilePicture(string newPictureUrl)
        {
            var currentUserId = this.userIdProvider.GetUserId();

            var currentUser = this.data.Users.All().FirstOrDefault(user => user.Id == currentUserId);
            currentUser.ProfilePictureUrl = newPictureUrl;
            this.data.SaveChanges();
        }
    }
}
