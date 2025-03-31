using AutoMapper;
using CarRentalPortal.Interfaces;
using CarRentalPortal.Models.DTO_s.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarRentalPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }



        #region Get User Profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var result = await _userService.GetUserProfileAsync(userId);

            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
        #endregion

        #region Get User's Cars
        [HttpGet("my-cars")]
        public async Task<IActionResult> GetMyCars()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var result = await _userService.GetUserCarsAsync(userId);

            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
        #endregion

        #region Get User's Rented Cars
        [HttpGet("rented-cars")]
        public async Task<IActionResult> GetRentedCars()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var result = await _userService.GetUserRentedCarsAsync(userId);

            return result.Success ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region Update Password
        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword([FromForm] UpdatePasswordDTO dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var result = await _userService.UpdatePasswordAsync(userId, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region Update Email
        [HttpPut("update-email")]
        public async Task<IActionResult> UpdateEmail([FromForm] UpdateUserEmailDTO dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var result = await _userService.UpdateUserEmailAsync(userId, dto.NewEmail);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region Update Phone Number
        [HttpPut("update-phone")]
        public async Task<IActionResult> UpdatePhone([FromForm] UpdateUserPhoneNumberDTO dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var result = await _userService.UpdateUserPhoneNumberAsync(userId, dto.NewPhoneNumber);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region Delete Account
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteOwnAccount()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var result = await _userService.DeleteOwnAccountAsync(userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        #endregion
    }
}
