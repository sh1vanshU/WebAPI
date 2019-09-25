using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Filters
{
    public class UserValidate
    {
        public static bool Login(string username, string password)
        {
            // create context object of entity framework database
            var context = new WebApiEntities();

            //find boolean value of user credentials are true or not
            bool isValidUser = context.RegistrationTables.Any(user => user.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && user.Password == password);

            // return boolean value
            return isValidUser;

        }
    }
}