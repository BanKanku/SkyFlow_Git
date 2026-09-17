using SkyFlow.Database;
using Xunit;

namespace SkyFlow.Tests
{
    public class UserRepositoryTests
    {
        [Fact]
        public void Authenticate_WithCorrectAdminCredentials_ReturnsAdminUser()
        {
            // Arrange
            UserRepository repository = new UserRepository();

            // Act
            var user = repository.Authenticate("admin", "admin123");

            // Assert
            Assert.NotNull(user);
            Assert.Equal("admin", user.Username);
            Assert.Equal("Admin", user.Role);
        }

        [Fact]
        public void Authenticate_WithIncorrectPassword_ReturnsNull()
        {
            // Arrange
            UserRepository repository = new UserRepository();

            // Act
            var user = repository.Authenticate("admin", "wrongpassword");

            // Assert
            Assert.Null(user);
        }

        [Fact]
        public void Authenticate_WithUnknownUsername_ReturnsNull()
        {
            // Arrange
            UserRepository repository = new UserRepository();

            // Act
            var user = repository.Authenticate(
                "unknown-user",
                "password123"
            );

            // Assert
            Assert.Null(user);
        }

        [Fact]
        public void Authenticate_UsernameIsCaseInsensitive()
        {
            // Arrange
            UserRepository repository = new UserRepository();

            // Act
            var user = repository.Authenticate("ADMIN", "admin123");

            // Assert
            Assert.NotNull(user);
            Assert.Equal("admin", user.Username);
        }
    }
}