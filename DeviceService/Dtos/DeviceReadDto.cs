namespace DeviceService.Dtos
{
    public class DeviceReadDto
    {
        public string Id { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public string Address { get; set; } = String.Empty;
        public int MaximumHourlyEnergyConsumption { get; set; }
        public string UserId { get; set; } = String.Empty;
    }
}
