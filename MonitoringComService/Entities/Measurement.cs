namespace MonitoringComService.Entities
{
    //Child
    public class Measurement
    {
        public int MeasurementId { get; set; }
        public long Timestamp { get; set; }
        public int MeasurementValue { get; set; }

        // Foreign key
        public int DeviceId { get; set; }

        // Navigation property
        public Device Device { get; set; } = null!; // Required reference navigation to principal
    }
}