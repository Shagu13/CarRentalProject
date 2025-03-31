using CarRentalPortal.Helpers;
using CarRentalPortal.Interfaces;
using CarRentalPortal.Models.DTO_s.Auth;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        #region Register User
        [HttpPost("Register")]
        public async Task<ServiceResponse<int>> Register([FromForm] UserRegisterDTO userRegisterDTO)
        {
            return await _authService.Register(userRegisterDTO);
        }
        #endregion

        #region User Login
        [HttpPost("Login")]
        public async Task<ServiceResponse<string>> Login([FromForm] UserLoginDTO userLoginDTO)
        {
            return await _authService.Login(userLoginDTO);
        }
        #endregion
    }
}
