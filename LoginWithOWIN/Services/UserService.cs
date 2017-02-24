using LoginWithOWIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginWithOWIN.Services
{
    public class UserService
    {


        public UserModel GetByUserId(int Id)
        {

            var user = GetUsers()
                        .Where(x => x.Id == Id)
                        .SingleOrDefault();

            return user;

        }


        private List<UserModel> GetUsers()
        {

            return new List<UserModel> {
                new UserModel { Id = 1, Email = "john@mail.com", Name = "John", Password = "john" },
                new UserModel { Id = 2, Email = "chris@mail.com", Name = "Chris", Password = "chris" },
                new UserModel { Id = 3, Email = "sam@mail.com", Name = "Sam", Password = "sam" },
                new UserModel { Id = 4, Email = "ellie@hotmail.com", Name = "Ellie", Password = "ellie" },
        };

        }



    }
}