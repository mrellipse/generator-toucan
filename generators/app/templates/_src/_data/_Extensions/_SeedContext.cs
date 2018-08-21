using System;
using System.Linq;
using <%=assemblyName%>.Data.Model;
using <%=assemblyName%>.Contract;
using <%=assemblyName%>.Contract.Security;

namespace <%=assemblyName%>.Data
{
    public static partial class Extensions
    {
        private const string AdminEmail = "webmaster@<%=assemblyName.toLowerCase()%>.org";

        public static void EnsureSeedData(this DbContextBase db, ICryptoService crypto)
        {
            EnsureLocalProvider(db);
            EnsureExternalProviders(db);
            User admin = EnsureAdmin(db, crypto);
            EnsureAuthorizationClaims(db, admin);
            EnsureSystemRoles(db, admin);
        }

        private static void EnsureAuthorizationClaims(DbContextBase db, User admin)
        {
            string[] claims = new string[]
            {
                SecurityClaimTypes.Example
            };

            foreach (string claim in claims)
            {
                var securityClaim = db.SecurityClaim.FirstOrDefault(o => o.SecurityClaimId == claim);

                if (securityClaim == null)
                {
                    securityClaim = new SecurityClaim()
                    {
                        CreatedBy = admin.UserId,
                        CreatedOn = DateTime.UtcNow,
                        Description = claim,
                        Enabled = true,
                        Origin = "System",
                        ValidationPattern = SecurityClaimTypes.AllowedValuesPattern,
                        SecurityClaimId = claim
                    };

                    db.SecurityClaim.Add(securityClaim);
                    db.SaveChanges();
                }
            }
        }

        private static Provider EnsureExternalProviders(DbContextBase db)
        {
            Provider provider = db.Provider.FirstOrDefault(o => o.ProviderId == ProviderTypes.Google);

            if (provider == null)
            {
                provider = new Provider()
                {
                    ProviderId = ProviderTypes.Google,
                    Name = "Google",
                    Description = "Logon using your google account",
                    Enabled = true
                };

                db.Provider.Add(provider);
                db.SaveChanges();
            }

            provider = db.Provider.FirstOrDefault(o => o.ProviderId == ProviderTypes.Microsoft);

            if (provider == null)
            {
                provider = new Provider()
                {
                    ProviderId = ProviderTypes.Microsoft,
                    Name = "Microsoft",
                    Description = "Logon using your microsoft account",
                    Enabled = true
                };

                db.Provider.Add(provider);
                db.SaveChanges();
            }

            return provider;
        }

        private static Provider EnsureLocalProvider(DbContextBase db)
        {
            Provider provider = db.Provider.FirstOrDefault(o => o.ProviderId == ProviderTypes.Local);

            if (provider == null)
            {
                provider = new Provider()
                {
                    ProviderId = ProviderTypes.Local,
                    Name = "Site",
                    Description = "Authenticate with a username/password provider by this site",
                    Enabled = true
                };

                db.Provider.Add(provider);
                db.SaveChanges();
            }

            return provider;
        }

        private static void EnsureSystemRoles(DbContextBase db, User admin)
        {
            foreach (var systemRole in RoleTypes.System)
            {
                Role role = db.Role.FirstOrDefault(o => o.RoleId == systemRole.Key);

                if (role == null)
                {
                    role = new Role()
                    {
                        CreatedBy = admin.UserId,
                        Enabled = true,
                        Name = systemRole.Value,
                        RoleId = systemRole.Key
                    };

                    if (systemRole.Key == RoleTypes.User)
                    {
                        var claim = db.SecurityClaim.SingleOrDefault(o => o.SecurityClaimId == SecurityClaimTypes.Example);
                        
                        if (claim != null)
                            role.SecurityClaims.Add(new RoleSecurityClaim()
                            {
                                Role = role,
                                SecurityClaimId = SecurityClaimTypes.Example,
                                Value = SecurityClaimValueTypes.Read.ToString()
                            });
                    }

                    db.Role.Add(role);
                    db.SaveChanges();
                }
            }
        }

        private static User EnsureAdmin(DbContextBase db, ICryptoService crypto)
        {
            User adminUser = db.User.SingleOrDefault(o => o.Username == AdminEmail);

            if (adminUser == null)
            {
                adminUser = new User()
                {
                    CultureName = "en",
                    DisplayName = "Webmaster",
                    Enabled = true,
                    TimeZoneId = Globalization.DefaultTimeZoneId,
                    Username = AdminEmail
                };

                db.User.Add(adminUser);
                db.SaveChanges();
            }

            Role adminRole = db.Role.FirstOrDefault(o => o.RoleId == RoleTypes.Admin);

            if (adminRole == null)
            {
                adminRole = new Role()
                {
                    CreatedByUser = adminUser,
                    Enabled = true,
                    Name = "Administrator",
                    RoleId = RoleTypes.Admin
                };
                db.Role.Add(adminRole);
                db.SaveChanges();
            }

            if (!db.UserRole.Any())
            {
                var userRole = new UserRole()
                {
                    Role = adminRole,
                    User = adminUser
                };

                string salt = crypto.CreateSalt();
                string hash = crypto.CreateKey(salt, "P@ssw0rd");

                var userProvider = new UserProviderLocal
                {
                    ProviderId = ProviderTypes.Local,
                    PasswordSalt = salt,
                    PasswordHash = hash,
                    User = adminUser,
                };

                db.UserRole.Add(userRole);
                db.UserProvider.Add(userProvider);

                db.SaveChanges();
            }

            return adminUser;
        }
    }
}