using DFDSVisitManagementAPI.Business.src.Services;
using DFDSVisitManagementAPI.Domain.src.DTOs.Auth;
using DFDSVisitManagementAPI.Domain.src.Entities;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Microsoft.Extensions.Configuration;

namespace DFDSVisitManagementAPI.Tests
{
    public class AuthServiceTests
    {
        private IConfiguration BuildConfig()
        {
            var inMemory = new Dictionary<string, string?>
            {
                { "Jwt:Key", "super-secret-key-that-is-long-enough-32chars" },
                { "Jwt:Issuer", "DFDSBackend" },
                { "Jwt:Audience", "DFDSClient" }
            };
            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemory)
                .Build();
        }

        [Fact]
        public async Task RegisterAsync_ReturnsNull_WhenPasswordsMismatch()
        {
            var userManager = Substitute.For<UserManager<AppUser>>(
                Substitute.For<IUserStore<AppUser>>(),
                null, null, null, null, null, null, null, null);

            var service = new AuthService(userManager, BuildConfig());

            var result = await service.RegisterAsync(new RegisterDto
            {
                FullName = "James Okafor",
                Email = "james@dfds.com",
                Password = "Password123!",
                ConfirmPassword = "WrongPassword!"
            });

            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_ReturnsNull_WhenUserNotFound()
        {
            var userManager = Substitute.For<UserManager<AppUser>>(
                Substitute.For<IUserStore<AppUser>>(),
                null, null, null, null, null, null, null, null);

            userManager.FindByEmailAsync(Arg.Any<string>())
                .Returns((AppUser?)null);

            var service = new AuthService(userManager, BuildConfig());

            var result = await service.LoginAsync(new LoginDto
            {
                Email = "nobody@dfds.com",
                Password = "Password123!"
            });

            Assert.Null(result);
        }
    }
}