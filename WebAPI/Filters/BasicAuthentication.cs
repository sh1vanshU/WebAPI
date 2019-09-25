using System;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WebAPI.Filters
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        private const string Realm = "My Realm";
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            //If the Authorization header is empty or null
            //then return Unauthorized
            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = actionContext.Request
                    .CreateResponse(HttpStatusCode.Unauthorized);

                // If the request was unauthorized, add the WWW-Authenticate header 
                // to the response which indicates that it require basic authentication
                if (actionContext.Response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    actionContext.Response.Headers.Add("WWW-Authenticate",
                        string.Format("Basic realm=\"{0}\"", Realm));
                }
            }
            else
            {
                //Get the authentication token from the request header
                string authenticationToken = actionContext.Request.Headers
                    .Authorization.Parameter;
                
                //Convert the string into an string array
                string[] Array = authenticationToken.Split('-');

                //First element of the array is the username
                string username = Array[0];

                //Second element of the array is the password
                string password = Array[1];

                //Third element of the array is the Creation of Time Stamp
                string timeStamp = Array[2];

                //Fourth element of the array is the user Id
                string id = Array[3];

                // Create Currnet timeStamp
                DateTime currentTimeStamp = DateTime.Now;

                //call the login method to check the username and password
                if (UserValidate.Login(username, password) && (currentTimeStamp - Convert.ToDateTime(timeStamp) < DateTime.Now.AddMinutes(30) - DateTime.Now))
                {
                    var identity = new GenericIdentity(id);

                    IPrincipal principal = new GenericPrincipal(identity, null);
                    Thread.CurrentPrincipal = principal;

                    if (HttpContext.Current != null)
                    {
                        HttpContext.Current.User = principal;
                    }
                }
                else
                {
                    actionContext.Response = actionContext.Request
                        .CreateResponse(HttpStatusCode.Unauthorized);
                }
            }
        }
    }
}