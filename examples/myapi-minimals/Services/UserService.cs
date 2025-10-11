using identity.server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using test_minimals.DTOs;
using test_minimals.infra.Models.Identity;
using test_minimals.Repository;

namespace test_minimals.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _usersRepository;
        private readonly IOptions<JwtConfiguration> _jwtOptions;

        public UserService(IRepository<User> usersRepository, IOptions<JwtConfiguration> jwtOptions)
        {
            _usersRepository = usersRepository;
            _jwtOptions = jwtOptions;
        }

        public Task<GeneralResponseDto> Register(RegisterDto model)
        {
            // Registration logic here
            var passwordHasher = new PasswordHasher<User>();
            var user = new User
            {
                UserName = model.Name,
                Email = model.Email,
                PasswordHash = passwordHasher.HashPassword(null, model.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _usersRepository.AddAsync(user);
            return Task.FromResult(new GeneralResponseDto
            {
                Status = 200,
                Description = "User registered successfully"
            });
        }
        public Task<UserDto> Login(LoginDto payload)
        {
            var user = _usersRepository.GetAllAsync<User>().Result.FirstOrDefault(u => u.UserName == payload.UserName || u.Email.Equals(payload.UserName));
            if (user == null)
            {
                return Task.FromResult(new UserDto()
                {
                    Id = string.Empty,
                    UserName = "User Not Found"
                });
            }
            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, payload.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return Task.FromResult(new UserDto()
                {
                    Id = string.Empty,
                    UserName = "Invalid Password"
                });
            }
            var claims = new[] { new Claim(ClaimTypes.Name, user.UserName) };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Value.Issuer,
                audience: _jwtOptions.Value.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtOptions.Value.ExpiryInMinutes),
                signingCredentials: creds
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return Task.FromResult(new UserDto
            {
                Id = user.Id.ToString(),
                UserName = user.UserName,
                Email = user.Email,
                Token = tokenString,
                TokenExpiry = token.ValidTo,
                IsActive = user.IsActive
            });
        }
    }
    public interface IUserService
    {
        Task<GeneralResponseDto> Register(RegisterDto model);
        Task<UserDto> Login(LoginDto payload);
    }
}
