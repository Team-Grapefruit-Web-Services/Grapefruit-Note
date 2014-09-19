using GrapefruitNote.Data;
using GrapefruitNote.DataTransferObjects;
using GrapefruitNote.Mappers;
using GrapefruitNote.Models;
using GrapefruitNote.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GrapefruitNote.Services.Controllers
{
    public class UserController : BaseApiController
    {
        private readonly Func<UserLoginModel, User> toUserEntity = UserMapper.ToUserEntity.Compile();

        public UserController() 
            : base(new GrapefruitNoteData())
        {
        }

        public UserController(IGrapefruitNoteData data) 
            : base(data)
        {
        }

        [HttpPost, ActionName("register")]
        public IHttpActionResult Register(UserLoginModel userToRegister)
        {
            //UserValidator.ValidateAuthCode(userToRegister.AuthCode);
            //UserValidator.ValidateUsername(userToRegister.Username);

            var newUser = toUserEntity(userToRegister);

            this.data.Users.Add(newUser);
            this.data.SaveChanges();

            return Ok();
        }

        [HttpPost, ActionName("login")]
        public IHttpActionResult Login(UserLoginModel userToLogin)
        {
            //UserValidator.ValidateAuthCode(userToLogin.AuthCode);
            //UserValidator.ValidateUsername(userToLogin.Username);

            var user = this.GetByUsernameAndAuthCode(userToLogin.Username, userToLogin.AuthCode);

            if (user == null)
            {
                return this.BadRequest("Invalid user data.");
            }

            user.SessionKey = UserValidator.GenerateSessionKey(user.UserId);
            this.data.SaveChanges();

            var userLoggedModel = new UserLoggedInModel()
            {
                Username = user.Username,
                SessionKey = user.SessionKey
            };

            return Ok(userLoggedModel);
        }

        [HttpGet, ActionName("logout")]
        public IHttpActionResult Logout(string sessionKey)
        {
            var currentUser = this.GetUserBySessionKey(sessionKey);

            if (currentUser == null)
            {
                return BadRequest("Invalid user");
            }

            currentUser.SessionKey = null;
            this.data.SaveChanges();

            return Ok("Logout successful.");
        }

        private User GetUserBySessionKey(string sessionKey)
        {
            return this.data.Users.All().FirstOrDefault(u => u.SessionKey == sessionKey);
        }

        private User GetByUsernameAndAuthCode(string username, string authCode)
        {
            return this.data.Users.All().FirstOrDefault(u => u.Username == username && u.AuthCode == authCode);
        }
    }
}