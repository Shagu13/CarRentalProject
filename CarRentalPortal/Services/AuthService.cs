using CarRentalPortal.Enums;
using CarRentalPortal.Helpers;
using CarRentalPortal.Interfaces;
using CarRentalPortal.Models;
using CarRentalPortal.Models.DTO_s.Auth;
using CarRentalPortal.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CarRentalPortal.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        #region Login
        public async Task<ServiceResponse<string>> Login(UserLoginDTO dto)
        {
            var response = new ServiceResponse<string>();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);

            if (user == null)
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.UserNotFound, dto.PhoneNumber);
                return response;
            }

            if (user.Status != UserStatus.Active)
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.UserNotActive);
                return response;
            }

            if (!VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.IncorrectPassword);
                return response;
            }

            var roles = new List<string> { "User" };

            var tokens = GenerateTokens(user, dto.StaySignedIn, roles);

            if (dto.StaySignedIn)
            {
                user.RefreshToken = tokens.RefreshToken;
                user.RefreshTokenExpirationDate = DateTime.Now.AddDays(2);
                await _context.SaveChangesAsync();
            }

            response.Data = tokens.AccessToken;
            response.Message = HelperService.GetResourcevalue(Constants.SuccessfulOperation);
            return response;
        }
        #endregion

        #region User Registration
        public async Task<ServiceResponse<int>> Register(UserRegisterDTO dto)
        {
            var response = new ServiceResponse<int>();

            if (await _context.Users.AnyAsync(u => u.PhoneNumber == dto.PhoneNumber || u.Email.ToLower() == dto.Email.ToLower()))
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.ExistingUser);
                return response;
            }

            CreatePasswordHash(dto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName.ToLower(),
                LastName = dto.LastName.ToLower(),
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email.ToLower(),
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Status = UserStatus.Active,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            response.Data = user.PhoneNumber;
            response.Message = HelperService.GetResourcevalue(Constants.SuccessfulOperation);
            return response;
        }
        #endregion

        #region Private Methods


        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt)) 
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private TokenDTO GenerateTokens(User user, bool staySignedIn, List<string>? roleNames)
        {
            string refreshToken = string.Empty;
            if (staySignedIn)
            {
                refreshToken = GenerateRefreshToken(user);
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpirationDate = DateTime.Now.AddDays(2);
            }

            var accessToken = GenerateAccessToken(user, roleNames);

            return new TokenDTO() { AccessToken = accessToken, RefreshToken = refreshToken };
        }

        private string GenerateAccessToken(User user, List<string>? roleNames)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim("custom:status", user.Status.ToString())
            };

            if (roleNames != null)
            {
                claims.AddRange(roleNames.Select(name => new Claim(ClaimTypes.Role, name)));
            }

            if (user.Email.ToLower() == "admin@gmail.com")
            {
                claims.Add(new Claim("admin", "true"));
            }


            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("JWTOptions:Secret").Value ?? string.Empty));

            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            double.TryParse(_config.GetSection("JWTOptions:accessTokenExpirationDays").Value, out double accessTokenExpirationDays);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(accessTokenExpirationDays),
                SigningCredentials = credentials,
                Issuer = _config.GetSection("JWTOptions:Issuer").Value,
                Audience = _config.GetSection("JWTOptions:Audience").Value
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            SecurityToken token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }

        private string GenerateRefreshToken(User user)
        {
            List<Claim> claims = new List<Claim>()
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.MobilePhone, user.PhoneNumber.ToString()),
        new Claim("custom:status", user.Status.ToString())
    };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("JWTOptions:Secret").Value ?? string.Empty));

            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            double.TryParse(_config.GetSection("JWTOptions:refreshTokenExpirationDays").Value, out double refreshTokenExpirationDays);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(refreshTokenExpirationDays),
                SigningCredentials = credentials,
                Issuer = _config.GetSection("JWTOptions:Issuer").Value,
                Audience = _config.GetSection("JWTOptions:Audience").Value
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            SecurityToken token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }
        #endregion
    }
}
