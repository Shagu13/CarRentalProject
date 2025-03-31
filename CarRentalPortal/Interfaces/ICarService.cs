using CarRentalPortal.Helpers;
using CarRentalPortal.Models.DTO_s.Car;

namespace CarRentalPortal.Interfaces
{
    public interface ICarService
    {
        Task<ServiceResponse<CarDTO>> AddCarAsync(Guid userId, AddCarDTO dto);
        Task<ServiceResponse<string>> RentCarAsync(Guid userId, RentCarDTO dto);
        Task<ServiceResponse<List<CarDTO>>> GetMostPopularCarsAsync(int top);
        Task<ServiceResponse<List<CarDTO>>> GetRandomCarsAsync(int count);
        Task<ServiceResponse<CarDTO>> UpdateCarAsync(Guid userId, UpdateCarDTO dto);
        Task<ServiceResponse<string>> DeleteCarAsync(Guid userId, Guid carId);
        Task<ServiceResponse<object>> SearchCarsByCityAsync(string city);
        Task<ServiceResponse<List<CarDTO>>> GetPartiallyAvailableCarsThisMonthAsync();


    }

}
