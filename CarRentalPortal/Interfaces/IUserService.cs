using CarRentalPortal.Helpers;
using CarRentalPortal.Models.DTO_s.Car;
using CarRentalPortal.Models.DTO_s.User;

namespace CarRentalPortal.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResponse<UserProfileDTO>> GetUserProfileAsync(Guid userId);
        Task<ServiceResponse<List<CarDTO>>> GetUserCarsAsync(Guid userId);
        Task<ServiceResponse<string>> UpdatePasswordAsync(Guid userId, UpdatePasswordDTO dto);
        Task<ServiceResponse<string>> UpdateUserEmailAsync(Guid userId, string newEmail);
        Task<ServiceResponse<string>> UpdateUserPhoneNumberAsync(Guid userId, int newPhoneNumber);
        Task<ServiceResponse<string>> DeleteOwnAccountAsync(Guid userId);
        Task<ServiceResponse<List<CarDTO>>> GetUserRentedCarsAsync(Guid userId);

    }
}
