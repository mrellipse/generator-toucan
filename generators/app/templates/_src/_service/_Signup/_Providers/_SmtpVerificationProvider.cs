using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using <%=assemblyName%>.Contract;
using <%=assemblyName%>.Data;
using <%=assemblyName%>.Data.Model;

namespace <%=assemblyName%>.Service
{
    public class SmtpVerificationProvider : IVerificationProvider
    {
        public SmtpVerificationProvider()
        {

        }

        public void Send(IUser recipient, string code)
        {
            
        }
    }
}
