using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using Omega.Plumbing.Data;
using OmegaInterop.Core;
using OmegaService.Core.Interface;

namespace OmegaService.Core.Data
{
    public class OmegaUserRepository : BaseRepository<CoreService>, IOmegaUserRepository
    {
        public OmegaUserRepository(IConnectionStringProvider connectionStringProvider) : base(connectionStringProvider)
        {
        }

        public int AddUser(OmegaUser user)
        {
            using var connection = new SqlConnection(ConnectionString);
            const string sql = "insert into [OmegaUser] (FirstName, LastName, Email) output inserted.Id values (@FirstName, @LastName, @Email)";
            return connection.QuerySingle<int>(sql, user);
        }

        public IEnumerable<OmegaUser> GetAllUsers()
        {
            using var connection = new SqlConnection(ConnectionString);
            return connection.Query<OmegaUser>("select * from [OmegaUser]").AsList();
        }
    }
}
