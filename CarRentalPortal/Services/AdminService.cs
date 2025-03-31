using CarRentalPortal.Enums;
using CarRentalPortal.Helpers;
using CarRentalPortal.Interfaces;
using CarRentalPortal.Models.DTO_s.Admin;
using CarRentalPortal.Models;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;

namespace CarRentalPortal.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Update User Status
        public async Task<ServiceResponse<string>> UpdateUserStatusAsync(UpdateUserStatusDTO dto)
        {
            var response = new ServiceResponse<string>();

            if (!IsSuperAdmin())
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.NotAuthorized);
                return response;
            }

            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.UserNotFound);
                return response;
            }

            if (!Enum.IsDefined(typeof(UserStatus), dto.Status))
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.WrongStatus);
                return response;
            }

            user.Status = dto.Status;
            await _context.SaveChangesAsync();

            response.Data = user.Status.ToString();
            response.Message = HelperService.GetResourcevalue(Constants.SuccessfulOperation);
            return response;
        }
        #endregion

        #region Change User Password
        public async Task<ServiceResponse<string>> ChangeUserPasswordAsync(ChangePasswordDTO dto)
        {
            var response = new ServiceResponse<string>();

            if (!IsSuperAdmin())
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.NotAuthorized);
                return response;
            }

            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.UserNotFound);
                return response;
            }

            using var hmac = new HMACSHA512();
            user.PasswordSalt = hmac.Key;
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.NewPassword));

            await _context.SaveChangesAsync();

            response.Message = HelperService.GetResourcevalue(Constants.SuccessfulOperation);
            return response;
        }
        #endregion

        #region Private Method
        private bool IsSuperAdmin()
        {
            var email = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
            return email != null && email.ToLower() == "admin@gmail.com";
        }
        #endregion
    }

}
