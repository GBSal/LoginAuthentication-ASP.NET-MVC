using LoginWithOWIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LoginWithOWIN.Extensions;

namespace LoginWithOWIN.Services
{
    public class UserService
    {

        private List<UserModel> _userList;
        private List<UserRole> _userRoles;


        public UserService()
        {

            if (_userList == null)
            {

                _userList = new List<UserModel>();
                _userList.Add(new UserModel { Id = 1, Email = "msal@mail.com", Name = "MSal", Password = "RR+k6E/DMcqlfigZ1mGK5SUHMB5vaNmtpuMWE2EUnvk=|~" });
            }

            if (_userRoles == null)
            {
                _userRoles = new List<UserRole>();
                _userRoles.Add(new UserRole { Id = 1, Name = "admin", UserId = 1 });
            }

        }
        public UserModel GetUserByEmail(string email)
        {

            var user = GetUsers()
                        .Where(x => x.Email == email)
                        .SingleOrDefault();

            return user;

        }
        public int AddUser(RegisterViewModel newUser)
        {
            var user = _userList.Where(x => x.Email.Equals(newUser.Email, StringComparison.InvariantCultureIgnoreCase));
            if (user.Any())
            {
                return 2;

                //2 User already exists 
            }
            var maxId = _userList
                        .OrderByDescending(u => u.Id)
                        .FirstOrDefault().Id;
            if (!newUser.Password.EndsWith(App.PasswordEncodingPostfix))
            {
                string pass = newUser.Password;
               var password = pass.EncodePassword(newUser.Email);
                newUser.Password = password;
            }
            _userList.Add(new UserModel {Id = maxId+=1, Email = newUser.Email, Name=newUser.Name, Password = newUser.Password });

            return 0;
        }
        public List<UserRole> GetByUserId(int Id)
        {

            var roles = GetUserRoles()
                            .Where(x => x.UserId == Id)
                            .ToList();

            return roles;
        }
        private List<UserModel> GetUsers()
        {
            return _userList;
        }
        private List<UserRole> GetUserRoles()
        {
            return _userRoles;
        }





    }
}