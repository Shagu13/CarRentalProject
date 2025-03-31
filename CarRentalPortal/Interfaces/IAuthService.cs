using CarRentalPortal.Helpers;
using CarRentalPortal.Models.DTO_s.Auth;

namespace CarRentalPortal.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<int>> Register(UserRegisterDTO dto);
        Task<ServiceResponse<string>> Login(UserLoginDTO dto);
    }
}
