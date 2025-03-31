using AutoMapper;
using CarRentalPortal.Enums;
using CarRentalPortal.Helpers;
using CarRentalPortal.Interfaces;
using CarRentalPortal.Models;
using CarRentalPortal.Models.DTO_s.Car;
using CarRentalPortal.Models.DTO_s.User;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;


namespace CarRentalPortal.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        #region Get User Cars
        public async Task<ServiceResponse<List<CarDTO>>> GetUserCarsAsync(Guid userId)
        {
            var response = new ServiceResponse<List<CarDTO>>();

            var user = await _context.Users.Include(u => u.Cars)
                                           .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.UserNotFound);
                return response;
            }

            var cars = _mapper.Map<List<CarDTO>>(user.Cars);
            response.Data = cars;
            response.Message = HelperService.GetResourcevalue(Constants.SuccessfulOperation);
            return response;
        }
        #endregion

        #region Get User Rented Cars
        public async Task<ServiceResponse<List<CarDTO>>> GetUserRentedCarsAsync(Guid userId)
        {
            var response = new ServiceResponse<List<CarDTO>>();

            var rentals = await _context.RentalRecords
                .Include(r => r.Car)
                .Where(r => r.UserId == userId)
                .ToListAsync();

            var rentedCars = rentals.Select(r => r.Car).Distinct().ToList();

            response.Data = _mapper.Map<List<CarDTO>>(rentedCars);
            response.Success = true;
            response.Message = rentedCars.Any()
                ? $"Found {rentedCars.Count} rented car(s)."
                : "You haven't rented any cars.";

            return response;
        }
        #endregion

        #region Get User Profile
        public async Task<ServiceResponse<UserProfileDTO>> GetUserProfileAsync(Guid userId)
        {
            var response = new ServiceResponse<UserProfileDTO>();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.UserNotFound);
                return response;
            }

            var profile = new UserProfileDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Status = user.Status.ToString()
            };

            response.Data = profile;
            response.Message = HelperService.GetResourcevalue(Constants.SuccessfulOperation);
            return response;
        }
        #endregion

        #region Update Password
        public async Task<ServiceResponse<string>> UpdatePasswordAsync(Guid userId, UpdatePasswordDTO dto)
        {
            var response = new ServiceResponse<string>();

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.UserNotFound);
                return response;
            }

            if (!VerifyPasswordHash(dto.OldPassword, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.IncorrectPassword);
                return response;
            }

            if (dto.NewPassword != dto.ConfirmNewPassword)
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.ConfirmNewPassword);
                return response;
            }

            if (dto.OldPassword == dto.NewPassword)
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.PasswordMustBeDifferent);
                return response;
            }

            CreatePasswordHash(dto.NewPassword, out byte[] newHash, out byte[] newSalt);
            user.PasswordHash = newHash;
            user.PasswordSalt = newSalt;

            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = HelperService.GetResourcevalue(Constants.SuccessfulOperation);
            return response;
        }


        #endregion

        #region Update User Email
        public async Task<ServiceResponse<string>> UpdateUserEmailAsync(Guid userId, string newEmail)
        {
            var response = new ServiceResponse<string>();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.UserNotFound);
                return response;
            }

            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == newEmail.ToLower() && u.Id != userId))
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.ExistingUser);
                return response;
            }

            user.Email = newEmail.ToLower();
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Data = user.Email;
            response.Message = HelperService.GetResourcevalue(Constants.SuccessfulOperation);
            return response;
        }
        #endregion

        #region Update Phone Number
        public async Task<ServiceResponse<string>> UpdateUserPhoneNumberAsync(Guid userId, int newPhoneNumber)
        {
            var response = new ServiceResponse<string>();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.UserNotFound);
                return response;
            }

            bool isPhoneTaken = await _context.Users.AnyAsync(u => u.PhoneNumber == newPhoneNumber && u.Id != userId);
            if (isPhoneTaken)
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.ExistingUser);
                return response;
            }

            user.PhoneNumber = newPhoneNumber;
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Data = user.PhoneNumber.ToString();
            response.Message = HelperService.GetResourcevalue(Constants.SuccessfulOperation);
            return response;
        }
        #endregion

        #region Delete User
        public async Task<ServiceResponse<string>> DeleteOwnAccountAsync(Guid userId)
        {
            var response = new ServiceResponse<string>();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.UserNotFound);
                return response;
            }

            if (user.Status == UserStatus.Deleted)
            {
                response.Success = false;
                response.Message = "Account is already deleted.";
                return response;
            }

            user.Status = UserStatus.Deleted;
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Data = user.Status.ToString();
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
        #endregion
    }
}
