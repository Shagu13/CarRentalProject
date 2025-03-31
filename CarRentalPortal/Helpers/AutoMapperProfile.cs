using AutoMapper;
using CarRentalPortal.Models.DTO_s.Car;
using CarRentalPortal.Models.Entities;

namespace CarRentalPortal.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AddCarDTO, Car>()
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore());


            CreateMap<Car, CarDTO>();
        }
    }
}
