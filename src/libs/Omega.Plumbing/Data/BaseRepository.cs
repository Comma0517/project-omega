using System;
using System.Collections.Generic;
using System.Reflection;

namespace Omega.Plumbing.Data
{
    public class BaseRepository<T> where T : ProjectOmegaService
    {
        protected string ConnectionString { get; }
        // ReSharper disable once StaticMemberInGenericType
        private static readonly Dictionary<Type, DbInfo> posTypeToDbInfoMap = new();

        protected BaseRepository(IConnectionStringProvider connectionStringProvider)
        {
            var posType = typeof(T);
            
            if (posTypeToDbInfoMap.ContainsKey(posType))
            {
                ConnectionString = connectionStringProvider.GetConnectionString(posTypeToDbInfoMap[posType].DbName);
                return;
            }

            var dbInfo = (DbInfo) posType.GetCustomAttribute(typeof(DbInfo)) ?? new DbInfo {DbName = OmegaGlobalConstants.DEFAULT_DB_NAME};
            ConnectionString = connectionStringProvider.GetConnectionString(dbInfo.DbName);
            
            posTypeToDbInfoMap[posType] = dbInfo;
        }

        public string GetConnectionString()
        {
            if (!ConnectionString.Contains("Database=Test"))
            {
                throw new ApplicationException("ConnectionString is only available outside the repository class when testing");
            }

            return ConnectionString;
        }
    }
}
