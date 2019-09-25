using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    public class ProfileModel
    {
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string DOB { get; set; }
        public string Language { get; set; }
        public string GenToken { get; set; }
    }
}