using Account.Domain;
using Account.Infrastructure.DataAccess;

namespace Account.Integration.Test
{
    [Collection("Database")]
    public class AccountRepositoryTest : DataContainer
    {
        [Fact]
        public async Task Account_Register_Login()
        {
            // Arrange
            await ResetDatabaseAsync();
            await using var context = CreateContext();
            var repository = new UserRepository(context);

            var user = new User() { 
                Id = Guid.NewGuid() ,
                Login = "test-login",
                PasswordHash = "passwd123",
                Role = RoleType.User
            };


            //Act
            await repository.Register(user);

            var result = await repository.Login(user.Login, user.PasswordHash);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
        }
    }
}
