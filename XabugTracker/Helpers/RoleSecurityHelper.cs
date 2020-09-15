//using Microsoft.AspNet.Identity;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace XabugTracker.Helpers
//{
//    public class RoleSecurityHelper
//    {
//        ProjectHelper projectHelper = new ProjectHelper();
//        string userId = HttpContext.Current.User.Identity.GetUserId();
//        public bool CanThisHttp(string Controller, string action, int Id)
//        {
//            if (action == "get" && Controller == "Project")
//            {
//                return projectHelper.IsUserOnProject(userId, Id);
//            }
//            else if (action == "post" && Controller == "Project")
//            {

//            }

//                case "Ticket": return;

//                default: return false;
//            }

//        }
//    }
//}