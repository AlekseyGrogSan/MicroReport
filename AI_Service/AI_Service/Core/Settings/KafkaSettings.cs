namespace AI_Service.Core.Settings
{
    public class KafkaSettings
    {
        public const string SectionName = "Kafka";
        public string BootstrapServers { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public Dictionary<string, string> Topics { get; set; } = new();
    }
}
