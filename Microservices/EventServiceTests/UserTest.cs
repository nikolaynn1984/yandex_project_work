using Account.Application;
using Account.Application.Abstractions.Repositories;
using Account.Application.Abstractions.Services;
using Account.Application.DTOs;
using Account.Domain.Entities;
using EventApplication.Abstractions.Services;
using EventApplication.Events.DTOs;
using EventDomain.Exceptions;
using EventInfrastructure.DataAccess;
using EventInfrastructure.DataAccess.Account;
using EventInfrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EventServiceTests
{
    public class UserTest
    {
        private readonly ServiceProvider serviceProvider;
        private readonly IServiceScope scope;
        private IUserService userService;
        private IUserRepository userRepository;

        public UserTest()
        {
            var dbName = Guid.NewGuid().ToString();
            var services = new ServiceCollection();
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(dbName));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IPasswordHashing, PasswordHashing>();
            services.AddScoped<IUserValidator, UserValidator>();

            services.Configure<JwtOptions>(options =>
            {
                options.Issuer = "Event";
                options.Audience = "Event";
                options.Expires = 30;
                options.Key = "sekret.key.Jwt.Token.10082026.yandex.learn";
            });

            this.serviceProvider = services.BuildServiceProvider();
            this.scope = this.serviceProvider.CreateScope();

            this.userService = this.scope.ServiceProvider.GetRequiredService<IUserService>();
            this.userRepository = this.scope.ServiceProvider.GetRequiredService<IUserRepository>();
        }

        [Fact]
        public async Task Users_Register_User()
        {
            var request = new RegisterRequest() {  Login = "testlogin", Password = "passwd123", Role = RoleType.User };

            await this.userService.Register(request);

            var user = await this.userRepository.GetByLogin(request.Login);

            Assert.True(user?.Login == request.Login);
        }

        [Fact]
        public async Task Users_RegisterUnique_Throws()
        {
            var request = new RegisterRequest() { Login = "testlogin2", Password = "passwd123", Role = RoleType.User };

            await this.userService.Register(request);


            var exception = await Assert.ThrowsAsync<ValidationException>(() => this.userService.Register(request));
            Assert.Equal($"Логин {request.Login} уже занят", exception.Message);
        }

        [Fact]
        public async Task Users_Login_Token()
        {
            var request = new RegisterRequest() { Login = "testloginlog", Password = "passwd123", Role = RoleType.User };

            await this.userService.Register(request);

            var result = await this.userService.Login(new LoginRequest() { Login =  request.Login, Password = request.Password });

            Assert.NotNull(result);
            
        }
    }
}
