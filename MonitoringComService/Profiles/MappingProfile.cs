using AutoMapper;
using MonitoringComService.DTOs;
using MonitoringComService.Entities;
using HashidsNet;
using System.Linq;

namespace MonitoringComService.Profiles
{
    public class MappingProfile : Profile
    {
        private readonly IHashids? _hashids;
        public MappingProfile()
        {
            _hashids = null;
        }
        public MappingProfile(IHashids hashids)
        {
            _hashids = hashids;
            // Mapping for Device
            // Source -> Target
            CreateMap<Device, DeviceDto>()
                .ForMember(dto => dto.DeviceId, opt => opt.MapFrom(entity => _hashids.Encode(entity.DeviceId)))
                .ForMember(dto => dto.UserId, opt => opt.MapFrom(entity => _hashids.Encode(entity.UserId)));

            CreateMap<DeviceDto, Device>()
                .ForMember(entity => entity.DeviceId, opt => opt.MapFrom(dto => _hashids.Decode(dto.DeviceId).FirstOrDefault()))
                .ForMember(entity => entity.UserId, opt => opt.MapFrom(dto => _hashids.Decode(dto.UserId).FirstOrDefault()));

            // Mapping for Measurement
            CreateMap<Measurement, MeasurementDto>()
                .ForMember(dto => dto.DeviceId, opt => opt.MapFrom(entity => _hashids.Encode(entity.DeviceId)));

            CreateMap<MeasurementDto, Measurement>()
                .ForMember(entity => entity.DeviceId, opt => opt.MapFrom(dto => _hashids.Decode(dto.DeviceId).FirstOrDefault()));
        }
    }
}
