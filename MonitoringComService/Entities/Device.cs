namespace MonitoringComService.Entities
{
    public class Device
    {
        public int DeviceId { get; set; }
        public int UserId { get; set; }
        public int MaxHourlyConsumption { get; set; }

        // Navigation property
        public ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();
    }

}