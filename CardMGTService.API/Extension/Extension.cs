using CardMGTService.Core.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CardMGTService.API.Extension
{
    public static class Extensions
    {
        public static string GetContentType(this string path)
        {
            var extension = path.Split('.')[1];

            if (extension == "jpg")
                return "image/jpg";

            if (extension == "jpeg")
                return "image/jpeg";

            if (extension == "gif")
                return "image/gif";

            if (extension == "mp4")
                return "video/mp4";

            return "image";
        }
        
        public static bool IsValidMediaType(this HttpPostedFileBase file)
        {
            var validTypes = new string[] { ".jpeg", ".jpg", "gif", ".png", ".mp4" };

            return validTypes.Any(vt => file.FileName.ToLower().EndsWith(vt));
        }
    }
}