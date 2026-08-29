namespace DocumentService.Core.Settings
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public Dictionary<string, string> Topics { get; set; } = new();
        public string GroupId { get; set; } = string.Empty;
    }
}
