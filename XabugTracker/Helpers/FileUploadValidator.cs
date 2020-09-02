using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace XabugTracker.Helpers
{
    public static class FileUploadValidator
    {
        public static bool IsWebFriendlyFile(HttpPostedFileBase file)
        {
            if (file == null)
                return false;
            if (file.ContentLength > 2 * 1024 * 1024 || file.ContentLength < 2048)
                return false;
            try
            {
                var incomingExt = Path.GetExtension(file.FileName);
                var allowableExtensions = WebConfigurationManager.AppSettings["AllowableExtensions"].Split(',');
                return allowableExtensions.Contains(incomingExt);
            }
            catch
            {
                return false;
            }
        }
    }
}