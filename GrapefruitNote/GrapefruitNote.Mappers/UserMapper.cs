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
                    
                };
            }
        }

        public static Expression<Func<UserModel,User>> ToUserEntity
        {
            get 
            {
                return userModel => new User
                {
                    
                };
            }
        }
    }
}
