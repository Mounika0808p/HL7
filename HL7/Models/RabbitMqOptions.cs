namespace HL7.Models
{
    public class RabbitMqOptions 
    
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string QueueName { get; set; }

        public string ExhangeName { get; set; }
        public string RoutingName { get; set; }
    }
}
