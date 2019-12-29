using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AIR_API
{
    public static class NewtonsoftValidator
    {
        public static bool ValidateJSON(this string s)
        {
            try
            {
                JToken.Parse(s);
                return true;
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}
