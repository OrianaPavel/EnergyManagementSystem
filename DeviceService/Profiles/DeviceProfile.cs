using AutoMapper;
using DeviceService.Dtos;
using DeviceService.Entities;
using HashidsNet;
using System.Linq;
using UserDevice.Dtos;
using UserService.Entities;

namespace DeviceService.Profiles
{
    public class DeviceProfile : Profile
    {
        private readonly IHashids? _hashids;
        public DeviceProfile()
        {
            _hashids = null;
        }
        public DeviceProfile(IHashids hashids)
        {
            _hashids = hashids;
            //Source -> Target
            // CreateDto -> Entity
            CreateMap<DeviceCreateDto, Device>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => _hashids.Decode(src.UserId).FirstOrDefault()))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore()); 

            // Entity -> ReadDto
            CreateMap<Device, DeviceReadDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => _hashids.Encode(src.Id)))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => _hashids.Encode(src.UserId)));
        }
    }
}
