using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XabugTracker.Models;

namespace XabugTracker.Helpers
{
    public class SeedHelper
    {
        Random random = new Random();
        public ApplicationUser RandomUserFromList(List<ApplicationUser> list)
        {
            return list[random.Next(list.Count)];
        }
    }
}