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
    public class ProfilePictureController : BaseApiController
    {
        private string GoogleDriveClientId = "735079433623-28dbigh1qehsp5k4tmpph2i3s40gr41t.apps.googleusercontent.com";
        private string GoogleDriveSecret = "2XzoOtuoNhV_AQogZnInr_ol";
        private string GoogleDriveClientName = "user";
        private string GoogleDriveApplicationName = "TestingGoogleDriveAPI";
        private string FileDefaultTitle = "GrapefruitNote";
        private string FileDefaultDescription = "A test document";

        private readonly IUserIdProvider userIdProvider;

        public ProfilePictureController()
            :this(new GrapefruitNoteData(), new AspNetUserIdProvider())
        {
        }

        public ProfilePictureController(IGrapefruitNoteData data, IUserIdProvider userIdProvider)
            :base(data)
        {
            this.userIdProvider = userIdProvider;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> PostProfilePicture()
        {
            if (!this.ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            //create credentials
            UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = this.GoogleDriveClientId,
                    ClientSecret = this.GoogleDriveSecret
                },
                new[] { DriveService.Scope.Drive },
                this.GoogleDriveClientName,
                CancellationToken.None).Result;

            // Create the service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = this.GoogleDriveApplicationName,
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
                    body.Title = this.FileDefaultTitle;
                    body.Description = this.FileDefaultDescription;
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
