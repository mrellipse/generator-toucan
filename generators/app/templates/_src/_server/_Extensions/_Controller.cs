using System;
using System.Linq;
using <%=assemblyName%>.Contract;
using Microsoft.AspNetCore.Mvc;

namespace <%=assemblyName%>.Server
{
    public static partial class Extensions
    {
        internal static string HttpContextCurrentUserKey = "CurrentUser";

        public static IUser ApplicationUser(this ControllerBase controller)
        {
            var user = controller.HttpContext.Items[HttpContextCurrentUserKey];

            return user == null ? null : (IUser)user;
        }
    }
}