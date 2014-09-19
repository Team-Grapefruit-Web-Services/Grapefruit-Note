namespace GrapefruitNote.Mappers
{
    using GrapefruitNote.DataTransferObjects;
    using GrapefruitNote.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity.EntityFramework;
    
    public class UserMapper
    {
        public static Expression<Func<User, UserModel>> ToUserModel
        {
            get 
            {
                return user => new UserModel
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    Categories = user.Categories.AsQueryable()
                        .Select(CategoryMapper.ToCategoryModel)
                        .ToList(),
                    Notes = user.Notes.AsQueryable()
                        .Select(NoteMapper.ToNoteModel)
                        .ToList()
                };
            }
        }

        public static Expression<Func<UserLoginModel, User>> ToUserEntity
        {
            get 
            {
                return userModel => new User
                {
                    Username = userModel.Username,
                    Email = userModel.Email,
                    AuthCode = userModel.AuthCode
                };
            }
        }
    }
}
