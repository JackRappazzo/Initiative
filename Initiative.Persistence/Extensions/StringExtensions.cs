using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Initiative.Persistence.Extensions
{
    public static class StringExtensions
    {
        public static bool IsValidObjectId(this string objectId)
        {
            if (string.IsNullOrEmpty(objectId) || objectId.Length != 24)
            {
                return false;
            }
            
            if(objectId == "000000000000000000000000")
            {
                return false; // This is the ObjectId for an empty document, which is not valid in this context.
            }

            return System.Text.RegularExpressions.Regex.IsMatch(objectId, "^[a-fA-F0-9]{24}$");
        }
    }
}
