using CarRentalPortal.Interfaces;
using CarRentalPortal.Models.DTO_s.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalPortal.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        #region Change User Status
        [HttpPut("update-user-status")]
        public async Task<IActionResult> UpdateUserStatus([FromForm] UpdateUserStatusDTO dto)
        {
            var result = await _adminService.UpdateUserStatusAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region Change User Password
        [HttpPut("change-user-password")]
        public async Task<IActionResult> ChangeUserPassword([FromForm] ChangePasswordDTO dto)
        {
            var result = await _adminService.ChangeUserPasswordAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        #endregion
    }
}
