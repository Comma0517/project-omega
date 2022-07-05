using System.Data.SqlClient;
using System.Linq;
using Dapper;
using FluentAssertions;
using Omega.Plumbing.Data;
using OmegaInterop.Core;
using OmegaService.Core.Data;
using Xunit;

namespace OmegaService.Core.Tests.Data
{
    public class OmegaUserRepositoryTest : BaseRepositoryTest
    {
        private OmegaUserRepository _omegaUserRepository;

        public OmegaUserRepositoryTest()
        {
            _omegaUserRepository = new OmegaUserRepository(new TestConnectionStringProvider(EnvSettings));
        }

        [Fact]
        public void GetAllUsers_NoUsers_ReturnsEmpty()
        {
            _omegaUserRepository.DeleteAllUsers();

            var actual = _omegaUserRepository.GetAllUsers().ToList();

            actual.Should().NotBeNull();
            actual.Should().HaveCount(0);
        }

        [Fact]
        public void GetAllUsers_UsersExist_UsersReturned()
        {
            _omegaUserRepository.DeleteAllUsers();
            var expected = new OmegaUser {FirstName = "Chuck", LastName = "Norris", Email = "chuck.norris@test.com"};
            var userId = _omegaUserRepository.AddUser(expected);
            expected.Id = userId;

            var actualUsers = _omegaUserRepository.GetAllUsers().ToList();

            actualUsers.Should().NotBeNull();
            actualUsers.Should().HaveCount(1);
            actualUsers[0].Should().BeEquivalentTo(expected);
        }
    }

    public static class OmegaUserRepositoryExtensions
    {
        public static void DeleteAllUsers(this OmegaUserRepository repo)
        {
            using var connection = new SqlConnection(repo.GetConnectionString());
            connection.Execute("delete from [OmegaUser]");
        }
    }
}
