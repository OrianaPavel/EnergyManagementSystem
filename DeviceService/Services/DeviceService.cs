using AutoMapper;
using DeviceService.Dtos;
using DeviceService.Entities;
using DeviceService.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using UserDevice.Dtos;

namespace DeviceService.Service
{
    public class DeviceService
    {
        private readonly IDeviceRepo _deviceRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<DeviceService> _logger;

        public DeviceService(IDeviceRepo deviceRepo, IMapper mapper, ILogger<DeviceService> logger)
        {
            _deviceRepo = deviceRepo ?? throw new ArgumentNullException(nameof(deviceRepo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public DeviceReadDto CreateDevice(DeviceCreateDto deviceCreateDto)
        {
            var device = _mapper.Map<Device>(deviceCreateDto);
            _deviceRepo.CreateDevice(ref device);

            return _mapper.Map<DeviceReadDto>(device);
        }

        public DeviceReadDto? GetDeviceById(int deviceId)
        {
            var device = _deviceRepo.GetDeviceById(deviceId);
            if (device == null)
            {
                _logger.LogWarning($"Device with ID {deviceId} was not found.");
                return null;
            }

            return _mapper.Map<DeviceReadDto>(device);
        }

        public IEnumerable<DeviceReadDto> GetAllDevices()
        {
            var devices = _deviceRepo.GetAllDevices();
            return _mapper.Map<IEnumerable<DeviceReadDto>>(devices);
        }

        public IEnumerable<DeviceReadDto> GetDevicesByUserId(int userId)
        {
            var devices = _deviceRepo.GetDevicesByUserId(userId);
            return _mapper.Map<IEnumerable<DeviceReadDto>>(devices);
        }

        public void UpdateDevice(int deviceId, DeviceCreateDto deviceUpdateDto)
        {
            var device = _deviceRepo.GetDeviceById(deviceId);
            if (device == null)
            {
                _logger.LogWarning($"Device with ID {deviceId} was not found during update.");
                return;
            }

            _mapper.Map(deviceUpdateDto, device);
            _deviceRepo.UpdateDevice(device);
        }

        public void DeleteDevice(int deviceId)
        {
            var device = _deviceRepo.GetDeviceById(deviceId);
            if (device == null)
            {
                _logger.LogWarning($"Device with ID {deviceId} was not found during delete.");
                return;
            }
            _deviceRepo.DeleteDevice(device);
        }
    }
}
