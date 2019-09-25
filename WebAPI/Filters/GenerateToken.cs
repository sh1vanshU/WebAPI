using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebAPI.Filters
{
    public class GenerateToken
    {
        internal string CreateToken(string username, string password, int UserID)
        {
            // create timeStamp for token
            DateTime timeStamp = DateTime.Now;

            // create generated token string 
            string generatedToken = username + "-" + password + "-" + timeStamp + "-" + UserID;

            //Return generated token
            return generatedToken;
        }
        
    }
}