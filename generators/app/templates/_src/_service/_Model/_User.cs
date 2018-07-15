﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using <%=assemblyName%>.Contract;

namespace <%=assemblyName%>.Service.Model
{
    public class User : IUserExtended
    {
        public User()
        {
        }
        
        public long UserId { get; set; }

        public string CultureName { get; set; }

        public string DisplayName { get; set; }

        public string Email
        {
            get
            {
                return this.Username;
            }
        }

        public bool Enabled { get; set; }
        
        public string TimeZoneId { get; set; }

        public string Username { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}
