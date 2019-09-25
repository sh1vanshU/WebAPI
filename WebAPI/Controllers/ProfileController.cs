using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Data.Entity.Migrations;
using WebAPI.Models;
using WebAPI.Filters;
using System.Threading;
using WebAPI;
using System.Web.Http.Cors;

namespace WebApi.Controllers
{
    //Enable Cros Origin 
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    //Apply Basic Authentication Attribute
    [BasicAuthentication]
    public class ProfileController : ApiController
    {
        [HttpGet]
        [Route("api/Profile/GetProfileData")]

        //Get Profile Data such as First Name , Last Name, Date Of Birth and Language by this get method of Profile controller.
        public HttpResponseMessage GetProfileData()
        {

            // Fetch User ID from Basic Authentication
            string ID = Thread.CurrentPrincipal.Identity.Name;

            //Check Whether model is valid or not. 
            //IF yes then pass the data from entity model(database) to Model class.
            if (ModelState.IsValid)
            {

                    // Create getProfileModel object of GetProfileModel class
                    GetProfileModel getProfileModel = new GetProfileModel();

                    //Query statement for specific ID.
                    var context = new WebApiEntities();
                    var query = from user in context.ProfileTables where ID == user.UserID.ToString() select user;

                    // Profile is the Class of
                    ProfileTable profile = query.FirstOrDefault();

                    // get profile Values
                    getProfileModel.Fname = profile.FirstName;
                    getProfileModel.Lname = profile.LastName;
                    getProfileModel.Language = profile.Language;
                    getProfileModel.DOB = profile.DOB.ToString();

                // Return getProfileModel class data with Ok status of HttpMessageResponse.
                return Request.CreateResponse(HttpStatusCode.OK, getProfileModel);

            }

            //Create Error Response if Model State is not valid and Send Message to user Please login again.
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "please login again");
        }
        
        [HttpPost]
        [Route("api/Profile/PostProfileData")]

        //Update or Save Profile Values Field in database
        public HttpResponseMessage PostProfileData(ProfileModel model)
        {

            //Check Whether Model state is valid or not.
            if (ModelState.IsValid)
            {
                
                var context = new WebApiEntities();

                //Create object of database Profile Table
                ProfileTable profile = new ProfileTable();

                // Split Token by - sign
                string[] tokenArray = model.GenToken.Split('-');

                // Get ID value by third element of tokenArray
                string ID = tokenArray[3];

                //convert string DOB into DateTime
                string inString = model.DOB;

                DateTime dateValue;
                if (!DateTime.TryParse(inString, out dateValue))
                {
                    Request.CreateErrorResponse(HttpStatusCode.PartialContent, "Please fill correct date of birth");
                }

                //Update profile Data for Existing User
                if (context.ProfileTables.Any(user => user.UserID.ToString() == ID))
                {
                    //Query Statement for Specific ID
                    var query = from user in context.ProfileTables where ID == user.UserID.ToString() select user.ID;
                    //Update Values
                    profile.FirstName = model.Fname;
                    profile.LastName = model.Lname;
                    profile.DOB = dateValue;
                    profile.UserID = Convert.ToInt32(ID);
                    profile.Language = model.Language;
                    profile.ID = query.First();

                    context.ProfileTables.Attach(profile);
                    context.Entry(profile).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();

                    //Return Message Profile Update with Ok status 
                    return Request.CreateResponse(HttpStatusCode.OK, "Profile Updated");

                }
                // Create New Profile data for New User
                else
                {
                    //Save new user value into database
                    profile.UserID = Convert.ToInt32(ID);
                    profile.FirstName = model.Fname;
                    profile.LastName = model.Lname;
                    profile.DOB = dateValue;
                    profile.Language = model.Language;

                    context.ProfileTables.Add(profile);
                    context.SaveChanges();

                    //Return Message Profile Save with Ok status 
                    return Request.CreateResponse(HttpStatusCode.OK, "Profile Save");
                }
            }
            // Create Error Response with Bad Request. and send message please fill again to client
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "please Fill again");
        }
        [HttpPost]
        [Route("api/Profile/PostProfileUpload")]
        //Upload Profile Picture of User
        public Task<HttpResponseMessage> PostProfileUpload()
        {
            // Fetch User ID from Basic Authentication
            string ID = Thread.CurrentPrincipal.Identity.Name;

            // Create string type of list for save sile path
            List<string> saveFilePath = new List<string>();

            //Check content is mime support ot not
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            //Find rootPath 
            string rootPath = HttpContext.Current.Server.MapPath("~/App_Data");

            //Create Provider object of MutipartFileStreamProvider
            var provider = new MultipartFileStreamProvider(rootPath);

            
            var task = Request.Content.ReadAsMultipartAsync(provider).ContinueWith<HttpResponseMessage>(t => 
            {
                if (t.IsCanceled || t.IsFaulted)
                {
                    Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception);
                }

                foreach (MultipartFileData item in provider.FileData)
                {
                    try
                    {
                        string name = item.Headers.ContentDisposition.FileName.Replace("\"", "");
                        string newFileName = ID + Path.GetExtension(name);

                        File.Move(item.LocalFileName, Path.Combine(rootPath, newFileName));
                        Uri baseuri = new Uri(Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.PathAndQuery, string.Empty));

                        string fileRelativePath = "~/App_Data/" + newFileName;
                        Uri fileFullPath = new Uri(baseuri, VirtualPathUtility.ToAbsolute(fileRelativePath));

                        saveFilePath.Add(fileFullPath.ToString());
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                return Request.CreateResponse(HttpStatusCode.Created, saveFilePath);
            });
            return task;
        }
        
        [HttpGet]
        [Route("api/Profile/GetProfilePic")]

        //Get Profile Picture for view
        public HttpResponseMessage GetProfilePic()
        {
            // Fetch User ID from Basic Authentication
            string ID = Thread.CurrentPrincipal.Identity.Name;

            // Create result object of HttpResponseMessage
            var result = new HttpResponseMessage(HttpStatusCode.OK);

            //Get All directory files
            string[] files = Directory.GetFiles(@"D:\repos\WebAPI\WebAPI\App_Data");

            //Create extension object of string type with initialisation
            string extensionOfFile = "";

            //Run loop for checking all files
            foreach(string file in files)
            {
                // split file into subfolders path
                string[] fileFolders = file.Split('\\');

                //get file name from path
                string fileName = fileFolders[5];

                //split filename by extension( . )
                string[] fileNameSplit = fileName.Split('.');

                //check whether filename matches with ID
                if(Path.GetFileName(fileNameSplit[0])==ID)
                {
                    // get extension from file if above condition is true
                    extensionOfFile = Path.GetExtension(file);
                }

            }

            //create file name.
            var filename = $"{ID}{extensionOfFile}";

            // find file path for specific user
            var filePath = HttpContext.Current.Server.MapPath($"~/App_Data/{filename}");

            //Replace forward slash with backward slash
            var newFilePath = filePath.Replace('/', '\\');
            
            //return  new file path to client with ok status code
            return Request.CreateResponse(HttpStatusCode.OK, newFilePath);
        }
        [HttpGet]
        [Route("api/Profile/GetDownloadPic")]

        //Download Picture Method
        public HttpResponseMessage GetDownloadPic()
        {

            //Get ID from Basic Authentication
            string ID = Thread.CurrentPrincipal.Identity.Name;

            // Create result object of HttpResponseMessage
            var result = new HttpResponseMessage(HttpStatusCode.OK);

            //Get All directory files
            string[] files = Directory.GetFiles(@"D:\repos\WebAPI\WebAPI\App_Data");

            //Create extension object of string type with initialisation
            string extensionOfFile = "";

            //Run loop for checking all files
            foreach (string file in files)
            {
                // split file into subfolders path
                string[] fileFolders = file.Split('\\');

                //get file name from path
                string fileName = fileFolders[5];

                //split filename by extension( . )
                string[] fileNameSplit = fileName.Split('.');

                //check whether filename matches with ID
                if (Path.GetFileName(fileNameSplit[0]) == ID)
                {
                    // get extension from file if above condition is true
                    extensionOfFile = Path.GetExtension(file);
                }
            }

            //Create File name
            var filename = $"{ID}{extensionOfFile}";

            // get File path for specific file name
            var filePath = HttpContext.Current.Server.MapPath($"~/App_Data/{filename}");

            // 1. Get the file in bytes
            var fileBytes = File.ReadAllBytes(filePath);

            // 2. convert file bytes into memory stream
            var fileMemoryStream = new MemoryStream(fileBytes);

            // 3. Add memory stream into response
            result.Content = new StreamContent(fileMemoryStream);

            // 4. Build the Response Header
            var header = result.Content.Headers;

            //Put attachment in header
            header.ContentDisposition = new ContentDispositionHeaderValue("Attachment");

            //put file Name in header
            header.ContentDisposition.FileName = filename;

            //Define content type of file
            header.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            // define content length
            header.ContentLength = fileMemoryStream.Length;

            //return result
            return result;
        }
    }
}