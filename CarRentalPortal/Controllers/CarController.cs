using CarRentalPortal.Helpers;
using CarRentalPortal.Interfaces;
using CarRentalPortal.Models.DTO_s.Car;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarRentalPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "OnlyActiveUsers")] 
    public class CarController : ControllerBase
    {
        private readonly ICarService _carService;

        public CarController(ICarService carService)
        {
            _carService = carService;
        }

        #region Add a Car
        [HttpPost("add-car")]
        public async Task<IActionResult> AddCar([FromForm] AddCarDTO dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var response = await _carService.AddCarAsync(userId, dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }
        #endregion

        #region Rent a Car
        [HttpPost("rent")]
        public async Task<IActionResult> RentCar([FromForm] RentCarDTO dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var result = await _carService.RentCarAsync(userId, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region Get Available Cars
        [AllowAnonymous]
        [HttpGet("available-this-month")]
        public async Task<IActionResult> GetAvailableCarsForCurrentMonth()
        {
            var result = await _carService.GetPartiallyAvailableCarsThisMonthAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region Search Cars by City 
        [AllowAnonymous]
        [HttpGet("search-by-city")]
        [SuppressModelStateInvalidFilter]
        public async Task<IActionResult> SearchByCity([FromQuery] string city)
        {
            var result = await _carService.SearchCarsByCityAsync(city);
            return result.Success ? Ok(result) : NotFound(result);
        }
        #endregion

        #region Get Popular Cars
        [AllowAnonymous]
        [HttpGet("most-popular")]
        public async Task<IActionResult> GetMostPopularCars([FromQuery] int top = 3)
        {
            var result = await _carService.GetMostPopularCarsAsync(top);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region Get Random Cars
        [AllowAnonymous]
        [HttpGet("random-car")]
        public async Task<IActionResult> GetRandomCars([FromQuery] int count = 3)
        {
            var result = await _carService.GetRandomCarsAsync(count);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region Update Car
        [HttpPut("update-car")]
        public async Task<IActionResult> UpdateCar([FromForm] UpdateCarDTO dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var result = await _carService.UpdateCarAsync(userId, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region Delete Car
        [HttpDelete("delete-car/{carId}")]
        public async Task<IActionResult> DeleteCar(Guid carId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var result = await _carService.DeleteCarAsync(userId, carId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        #endregion
    }
}
