using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using <%=assemblyName%>.Common;
using <%=assemblyName%>.Data.Model;

namespace <%=assemblyName%>.Data
{
    public abstract class DbContextBase : DbContext
    {
        public virtual DbSet<Provider> Provider { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserProvider> UserProvider { get; set; }
        public virtual DbSet<UserProviderLocal> LocalProvider { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<Verification> Verification { get; set; }

        private Config designTimeConfig;
        protected Config DesignTimeConfig
        {
            get
            {
                if (designTimeConfig == null)
                {
                    try
                    {
                        DirectoryInfo info = new DirectoryInfo(AppContext.BaseDirectory);
                        DirectoryInfo dataProjectRoot = info.Parent.Parent.Parent.Parent;

                        string basePath = Path.Combine(dataProjectRoot.FullName, "data");

                        IConfigurationRoot config = new ConfigurationBuilder()
                            .SetBasePath(basePath)
                            .AddJsonFile(<%if(dbProvider == 'npgsql'){%>"npgsql.json"<%}%><%if(dbProvider == 'mssql'){%>"mssql.json"<%}%>)
                            .Build();

                        designTimeConfig = config.GetTypedSection<Config>("data");
                    }
                    catch (Exception) { }
                }
                return designTimeConfig;
            }
        }

        public DbContextBase() : base()
        {

        }

        public DbContextBase(DbContextOptions options) : base(options)
        {

        }

        protected virtual void BeforeModelCreated(ModelBuilder modelBuilder)
        {
            string schemaName = this.DesignTimeConfig?.SchemaName;

            if (!string.IsNullOrWhiteSpace(schemaName))
                modelBuilder.HasDefaultSchema(schemaName);
        }

        protected virtual void AfterModelCreated(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // set all string data types to unicode
                foreach (var prop in entity.GetProperties())
                {
                    if (prop.ClrType == typeof(string))
                        prop.IsUnicode(true);
                }

                // disable cascade delete operations
                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    foreach (var relationship in entityType.GetForeignKeys())
                    {
                        relationship.DeleteBehavior = DeleteBehavior.Restrict;
                    }
                }
            }
        }
    }
}