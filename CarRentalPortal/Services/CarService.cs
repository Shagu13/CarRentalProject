using CarRentalPortal.Helpers;
using CarRentalPortal.Models.DTO_s.Car;
using CarRentalPortal.Models;
using CarRentalPortal.Interfaces;
using Microsoft.EntityFrameworkCore;
using CarRentalPortal.Models.Entities;
using AutoMapper;
using CarRentalPortal.Enums;


namespace CarRentalPortal.Services
{
    public class CarService : ICarService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CarService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region Add Car
        public async Task<ServiceResponse<CarDTO>> AddCarAsync(Guid userId, AddCarDTO dto)
        {
            var response = new ServiceResponse<CarDTO>();

            var user = await _context.Users.Include(u => u.Cars)
                                           .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.UserNotFound);
                response.Data = null; 
                return response;
            }


            var imagePaths = new List<string>();

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            if (dto.Images != null && dto.Images.Count > 0)
            {
                foreach (var image in dto.Images.Take(3))
                {
                    if (image.Length > 0)
                    {
                        var uniqueFileName = $"{Guid.NewGuid()}_{image.FileName}";
                        var filePath = Path.Combine(uploadPath, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        imagePaths.Add($"/uploads/{uniqueFileName}");
                    }
                }
            }

            if (dto.EngineType == EngineType.Electric && (!dto.RangeInKm.HasValue || dto.RangeInKm <= 0))
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.RangeInKm);
                return response;
            }

            if (dto.EngineType != EngineType.Electric && (!dto.FuelTankCapacity.HasValue || dto.FuelTankCapacity <= 0))
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.FuelTankCapacity);
                return response;
            }

            var car = new Car
            {
                CarBrand = dto.CarBrand,
                CarModel = dto.CarModel,
                YearOfManufacture = dto.YearOfManufacture,
                DailyRentalPrice = dto.DailyRentalPrice,
                CarCapacity = dto.CarCapacity,
                UserPhoneNumber = dto.UserPhoneNumber,
                City = dto.City,
                TransmissionType = dto.TransmissionType,  
                EngineType = dto.EngineType,
                FuelTankCapacity = dto.FuelTankCapacity,
                RangeInKm = dto.RangeInKm,
                UserId = userId,
                Images = imagePaths
            };


            user.Cars.Add(car);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<CarDTO>(car);

            response.Success = true;
            response.Data = result;
            response.Message = HelperService.GetResourcevalue(Constants.SuccessfulOperation);
            return response;
        }
        #endregion

        #region Get Most Popular Cars
        public async Task<ServiceResponse<List<CarDTO>>> GetMostPopularCarsAsync(int top)
        {
            var response = new ServiceResponse<List<CarDTO>>();

            if (top <= 0)
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.PositiveNumber);
                return response;
            }

            var popularCars = await _context.Cars
                .OrderByDescending(c => c.RentalCount)
                .Take(top)
                .ToListAsync();

            if (!popularCars.Any())
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.CarNotFound);
                return response;
            }

            response.Data = _mapper.Map<List<CarDTO>>(popularCars);
            response.Success = true;
            response.Message = $"Top {top} most popular car(s) retrieved.";
            return response;
        }
        #endregion

        #region Get Random Cars
        public async Task<ServiceResponse<List<CarDTO>>> GetRandomCarsAsync(int count)
        {
            var response = new ServiceResponse<List<CarDTO>>();

            var totalCars = await _context.Cars.CountAsync();
            if (totalCars == 0)
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.CarNotFound);
                return response;
            }

            if (count <= 0)
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.PositiveNumber);
                return response;
            }

            var random = new Random();

            var allCars = await _context.Cars.ToListAsync();
            var shuffledCars = allCars.OrderBy(x => random.Next()).Take(count).ToList();

            response.Data = _mapper.Map<List<CarDTO>>(shuffledCars);
            response.Success = true;
            response.Message = $"Returned {shuffledCars.Count} random car(s).";
            return response;
        }
        #endregion

        #region Search Cars By City       
        public async Task<ServiceResponse<object>> SearchCarsByCityAsync(string? city)
        {
            var response = new ServiceResponse<object>();

            if (string.IsNullOrWhiteSpace(city))
            {
                var cities = await _context.Cars
                    .Select(c => c.City)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync();

                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.EmptyCityField);
                response.Data = cities;
                return response;
            }

            var cars = await _context.Cars
                .Where(c => c.City.ToLower().StartsWith(city.ToLower()))
                .ToListAsync();

            var carDTOs = _mapper.Map<List<CarDTO>>(cars);

            response.Data = carDTOs;
            response.Success = carDTOs.Any();
            response.Message = carDTOs.Any()
                ? $"Found {carDTOs.Count} car(s) in cities starting with '{city}'."
                : "No cars found for this city.";

            return response;
        }
        #endregion

        #region Get Available Cars
        public async Task<ServiceResponse<List<CarDTO>>> GetPartiallyAvailableCarsThisMonthAsync()
        {
            var response = new ServiceResponse<List<CarDTO>>();

            var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var allCars = await _context.Cars.ToListAsync();

            var availableCars = new List<Car>();

            foreach (var car in allCars)
            {
                var rentals = await _context.RentalRecords
                    .Where(r => r.CarId == car.Id &&
                                r.RentalStartDate <= endOfMonth &&
                                r.RentalEndDate >= startOfMonth)
                    .ToListAsync();

                if (!IsFullyBooked(startOfMonth, endOfMonth, rentals))
                {
                    availableCars.Add(car);
                }
            }

            response.Data = _mapper.Map<List<CarDTO>>(availableCars);
            response.Message = $"{availableCars.Count} car(s) are available for at least some days this month.";
            return response;
        }

        private bool IsFullyBooked(DateTime monthStart, DateTime monthEnd, List<RentalRecord> rentals)
        {
            var bookedDays = new HashSet<DateTime>();

            foreach (var rental in rentals)
            {
                var start = rental.RentalStartDate < monthStart ? monthStart : rental.RentalStartDate;
                var end = rental.RentalEndDate > monthEnd ? monthEnd : rental.RentalEndDate;

                for (var date = start.Date; date <= end.Date; date = date.AddDays(1))
                {
                    bookedDays.Add(date);
                }
            }

            var totalDaysInMonth = (monthEnd - monthStart).Days + 1;
            return bookedDays.Count >= totalDaysInMonth;
        }
        #endregion

        #region Rent Car
        public async Task<ServiceResponse<string>> RentCarAsync(Guid userId, RentCarDTO dto)
        {
            var response = new ServiceResponse<string>();

            var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == dto.CarId);
            if (car == null)
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.CarNotFound);
                return response;
            }

            if (car.UserId == userId)
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.OwnCarRentError);
                return response;
            }

            DateTime startDate = dto.StartDate.ToDateTime(TimeOnly.MinValue);
            DateTime endDate = dto.EndDate.ToDateTime(TimeOnly.MaxValue);

            if (startDate.Date < DateTime.Today)
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.DateError);
                return response;
            }

            if (endDate < startDate)
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.DateError);
                return response;
            }

            var overlappingRentals = await _context.RentalRecords
                .Where(r => r.CarId == dto.CarId &&
                           ((startDate >= r.RentalStartDate && startDate <= r.RentalEndDate) ||
                            (endDate >= r.RentalStartDate && endDate <= r.RentalEndDate) ||
                            (startDate <= r.RentalStartDate && endDate >= r.RentalEndDate)))
                .ToListAsync();

            if (overlappingRentals.Any())
            {
                var conflictingDates = overlappingRentals
                    .Select(r => $"{r.RentalStartDate:yyyy-MM-dd} → {r.RentalEndDate:yyyy-MM-dd}")
                    .ToList();

                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.CarAlreadyRented, car.CarBrand, car.CarModel, string.Join(", ", conflictingDates));
                return response;
            }

            int rentalDays = (endDate.Date - startDate.Date).Days + 1;

            var rental = new RentalRecord
            {
                CarId = car.Id,
                UserId = userId,
                RentalStartDate = startDate,
                RentalEndDate = endDate,
            };

            car.RentalCount += 1;
            _context.RentalRecords.Add(rental);
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = $"Car '{car.CarBrand} {car.CarModel}' rented for {rentalDays} days.";
            return response;
        }
        #endregion

        #region Update Car
        public async Task<ServiceResponse<CarDTO>> UpdateCarAsync(Guid userId, UpdateCarDTO dto)
        {
            var response = new ServiceResponse<CarDTO>();
            var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == dto.Id && c.UserId == userId);

            if (car == null)
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.CarNotFound);
                return response;
            }

            car.CarBrand = dto.CarBrand;
            car.CarModel = dto.CarModel;
            car.YearOfManufacture = dto.YearOfManufacture;
            car.DailyRentalPrice = dto.DailyRentalPrice;
            car.CarCapacity = dto.CarCapacity;
            car.City = dto.City;
            car.FuelTankCapacity = dto.FuelTankCapacity;

            if (dto.Images != null && dto.Images.Count > 0)
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                if (car.Images != null)
                {
                    foreach (var oldImage in car.Images)
                    {
                        var oldImagePath = Path.Combine("wwwroot", oldImage.TrimStart('/'));
                        if (File.Exists(oldImagePath))
                        {
                            File.Delete(oldImagePath);
                        }
                    }
                }

                var newImagePaths = new List<string>();
                foreach (var image in dto.Images.Take(3))
                {
                    if (image.Length > 0)
                    {
                        var fileName = $"{Guid.NewGuid()}_{image.FileName}";
                        var path = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        newImagePaths.Add($"/uploads/{fileName}");
                    }
                }

                car.Images = newImagePaths;
            }

            await _context.SaveChangesAsync();
            response.Data = _mapper.Map<CarDTO>(car);
            response.Message = HelperService.GetResourcevalue(Constants.SuccessfulOperation);
            return response;
        }
        #endregion

        #region Delete Car
        public async Task<ServiceResponse<string>> DeleteCarAsync(Guid userId, Guid carId)
        {
            var response = new ServiceResponse<string>();

            var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == carId && c.UserId == userId);
            if (car == null)
            {
                response.Success = false;
                response.Message = HelperService.GetResourcevalue(Constants.CarNotFound);
                return response;
            }

            foreach (var imagePath in car.Images)
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = HelperService.GetResourcevalue(Constants.SuccessfulOperation);
            return response;
        }
        #endregion
    }
}
