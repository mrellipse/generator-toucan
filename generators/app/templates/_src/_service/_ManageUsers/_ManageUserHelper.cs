using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using <%=assemblyName%>.Contract;
using <%=assemblyName%>.Data;
using <%=assemblyName%>.Data.Model;

namespace <%=assemblyName%>.Service.Helpers
{
    public class ManageUserHelper
    {
        private readonly User user;
        public ManageUserHelper(User user)
        {
            this.user = user;
        }
        
        public ManageUserHelper UpdateCulture(IUserExtended user){

            this.user.CultureName = user.CultureName;
            this.user.TimeZoneId = user.TimeZoneId;

            return this;
        }

        public ManageUserHelper UpdateCulture(string cultureName, string timeZoneId){

            this.user.CultureName = cultureName;
            this.user.TimeZoneId = timeZoneId;

            return this;
        }

        public ManageUserHelper UpdateProfile(IUserExtended user)
        {
            this.user.DisplayName = user.DisplayName;

            return this;
        }

        public ManageUserHelper UpdateStatus(bool enabled, bool verified)
        {
            this.user.Enabled = enabled;
            this.user.Verified = verified;
            
            return this;
        }
    }
}