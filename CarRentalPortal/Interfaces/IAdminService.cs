using CarRentalPortal.Helpers;
using CarRentalPortal.Models.DTO_s.Admin;

namespace CarRentalPortal.Interfaces
{
    public interface IAdminService
    {
        Task<ServiceResponse<string>> UpdateUserStatusAsync(UpdateUserStatusDTO dto);
        Task<ServiceResponse<string>> ChangeUserPasswordAsync(ChangePasswordDTO dto);
    }
}
