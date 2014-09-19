namespace GrapefruitNote.Services.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using Google;
    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Drive.v2;
    using Google.Apis.Drive.v2.Data;
    using Google.Apis.Services;

    using GrapefruitNote.Services.Controllers;
    using GrapefruitNote.Data;
    using GrapefruitNote.Models;

    public class CloudStorageController : BaseApiController
    {
        private const string GoogleDriveClientId = "735079433623-28dbigh1qehsp5k4tmpph2i3s40gr41t.apps.googleusercontent.com";
        private const string GoogleDriveSecret = "2XzoOtuoNhV_AQogZnInr_ol";
        private const string GoogleDriveClientName = "user";
        private const string GoogleDriveApplicationName = "TestingGoogleDriveAPI";
        private const string FileDefaultTitle = "GrapefruitNote";
        private const string FileDefaultDescription = "A test document";
        private const string GoogleDriveRootId = "0AK3dUzJZtTtPUk9PVA";
        private const string GoogleDrivePublicFOlderId = "0B63dUzJZtTtPa0FrT3JjcnE1RVk";

        public CloudStorageController()
            : this(new GrapefruitNoteData())
        {
        }

        public CloudStorageController(IGrapefruitNoteData data)
            : base(data)
        {
        }

        [HttpPost]
        public async Task<HttpResponseMessage> UploadPicture(string sessionKey)
        {
            Models.User currentUser = this.GetUserBySessionKey(sessionKey);

            if (currentUser == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid user");
            }

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
                    body.Parents = new List<ParentReference>() { new ParentReference(){ Id = GoogleDrivePublicFOlderId}};

                    byte[] byteArray = item.ReadAsByteArrayAsync().Result;

                    System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);

                    FilesResource.InsertMediaUpload request = service.Files.Insert(body, stream, item.Headers.ContentType.MediaType);
                    request.Upload();

                    File file = request.ResponseBody;
                    string newPictureUrl = file.WebContentLink.Substring(0, file.WebContentLink.Length - 16);
                    var wtf = file.AlternateLink;
                    //Change profile picture in DB.
                    this.SetUserProfilePicture(currentUser, newPictureUrl);

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

        private void SetUserProfilePicture(Models.User currentUser, string newPictureUrl)
        {
            currentUser.ProfilePictureUrl = newPictureUrl;
            this.data.SaveChanges();
        }

        private GrapefruitNote.Models.User GetUserBySessionKey(string sessionKey)
        {
            return this.data.Users.All().FirstOrDefault(u => u.SessionKey == sessionKey);
        }
    }
}
