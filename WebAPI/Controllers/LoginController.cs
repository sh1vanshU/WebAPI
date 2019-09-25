using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebAPI.Filters;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LoginController : ApiController
    {
        [HttpPost]
        [Route("api/Login/UserLogin")]
        public HttpResponseMessage UserLogin(LoginModel model)
        {
            // Check Model State is valid or not
            if (!ModelState.IsValid)
            {
                return null;
            }
            using (var context = new WebApiEntities())
            {

                bool isValidUser = context.RegistrationTables.Any(user => user.Username.Equals(model.Username, StringComparison.OrdinalIgnoreCase) && user.Password == model.Password);
                // Check whether the user is valid or not
                if (isValidUser)
                {
                    // Run Query for user selection where username are matched
                    var query = from user in context.RegistrationTables where user.Username == model.Username select user;
                    //Get first Default query Value
                    RegistrationTable userDetails = query.FirstOrDefault();
                    //create object of Generate Token Class
                    GenerateToken generateToken = new GenerateToken();
                    // Return generatedToken through CreateToken Method
                    string generatedToken = generateToken.CreateToken(userDetails.Username, userDetails.Password, userDetails.UserID);

                    // Update token value in  Database
                    if (context.RegistrationTables.Any(user => user.UserID == userDetails.UserID))
                    {
                        RegistrationTable registration = new RegistrationTable();
                        registration.Token = generatedToken;
                        registration.UserID = userDetails.UserID;
                        registration.Username = userDetails.Username;
                        registration.Password = userDetails.Password;
                        // Add or Update Database context
                        context.RegistrationTables.AddOrUpdate(registration);
                        context.SaveChanges();

                    }
                    // Return generated token to client
                    return Request.CreateResponse(HttpStatusCode.OK, generatedToken);
                }
                //If Usernama and Password are invalid then return invalid username
                ModelState.AddModelError("", "Invalid username and password or Token Expired -- Please Login Again");
                // return request status for create error response
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }


    }
}
