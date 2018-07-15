using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using <%=assemblyName%>.Contract;
using <%=assemblyName%>.Data;
using <%=assemblyName%>.Data.Model;
using <%=assemblyName%>.Service.Helpers;
using <%=assemblyName%>.Service.Model;

namespace <%=assemblyName%>.Service
{
    public class ManageUserService : IManageUserService, IManageProfileService
    {
        private readonly DbContextBase db;

        public ManageUserService(DbContextBase db)
        {
            this.db = db;
        }

        public async Task<IDictionary<string, string>> GetAvailableRoles()
        {
            var q = (from r in this.db.Role
                     where r.Enabled
                     orderby r.Name ascending
                     select r);

            return await q.AsNoTracking().ToDictionaryAsync(o => o.RoleId, o => o.Name);
        }

        public async Task<IUserExtended> ResolveUserBy(long userId)
        {
            Data.Model.User dbUser = this.ResolveUser(userId);

            return await Task.FromResult(this.MapUser(dbUser));
        }

        public async Task<IUserExtended> ResolveUserBy(string username)
        {
            Data.Model.User dbUser = this.ResolveUser(username);

            return await Task.FromResult(this.MapUser(dbUser));
        }

        public Task<ISearchResult<IUserExtended>> Search(int page, int pageSize)
        {
            var q = from u in this.db.User.Include(o => o.Roles).Include(o => o.Providers)
                    select u;

            var result = (ISearchResult<IUserExtended>)new SearchResult<IUserExtended>(q.AsNoTracking(), o => MapUser(o as Data.Model.User), page, pageSize);

            return Task.FromResult(result);
        }

        public async Task<IUserExtended> UpdateUser(IUserExtended user)
        {
            Data.Model.User dbUser = this.ResolveUser(user.Username);

            if (dbUser == null)
                return null;

            new ManageUserHelper(dbUser)
                .UpdateCulture(user)
                .UpdateProfile(user);

            this.db.UpdateUserRoles(dbUser, user.Roles.ToArray());

            await this.db.SaveChangesAsync();

            return this.MapUser(dbUser);
        }
        public async Task<IUserExtended> UpdateUserCulture(long userId, string cultureName, string timeZoneId)
        {
            Data.Model.User dbUser = this.ResolveUser(userId);

            if (dbUser == null)
                return null;

            new ManageUserHelper(dbUser).UpdateCulture(cultureName, timeZoneId);

            await this.db.SaveChangesAsync();

            return this.MapUser(dbUser);
        }
        public async Task<IUserExtended> UpdateUserStatus(string username, bool enabled)
        {
            Data.Model.User dbUser = this.ResolveUser(username);

            if (dbUser == null)
                return null;

            new ManageUserHelper(dbUser).UpdateStatus(enabled);

            await this.db.SaveChangesAsync();

            return this.MapUser(dbUser);
        }

        private Data.Model.User ResolveUser(string username)
        {
            return this.db.User
                .Include(o => o.Roles)
                .Include(o => o.Providers)
                .Include(o => o.Verifications)
                .SingleOrDefault(o => o.Username == username);
        }

        private Data.Model.User ResolveUser(long userId)
        {
            return this.db.User
                .Include(o => o.Roles)
                .Include(o => o.Providers)
                .Include(o => o.Verifications)
                .SingleOrDefault(o => o.UserId == userId);
        }

        private Model.User MapUser(Data.Model.User user)
        {
            if (user == null)
                return new Model.User();
            else
                return new Model.User()
                {
                    CultureName = user.CultureName,
                    DisplayName = user.DisplayName,
                    Enabled = user.Enabled,
                    Roles = user.Roles.Select(o => o.RoleId),
                    TimeZoneId = user.TimeZoneId,
                    UserId = user.UserId,
                    Username = user.Username
                };
        }
    }
}