using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Filters
{
    public class ValidateToken
    {
        internal int ID(string genToken)
        {
            // split gentoken string into string array for Fetching ID
            string[] array = genToken.Split('-');

            // return  ID from Gentoken String
            return Convert.ToInt32(array[3]);
        }
    }
}