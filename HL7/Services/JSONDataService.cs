using System.Text;
using System.Xml;
using HL7.Models;
using HL7.Services.Interfaces;
using Microsoft.AspNetCore.Connections;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace HL7.Services
{
    public class JSONDataService : IInsertHl7DataService
    {

        string _connectionString;
        private readonly RabbitMqOptions _rabbitMqOptions;
        IConfiguration _configuration;
        IConfigurationSection applicationSettings;
        public JSONDataService(IConfiguration configuration ) // Inject dependencies
        {
            _configuration = configuration;
            _rabbitMqOptions = _rabbitMqOptions = configuration.GetSection("RabbitMQ").Get<RabbitMqOptions>();
            this.applicationSettings = configuration.GetSection("ConnectionStrings");
            _connectionString = applicationSettings.GetValue<string>("DefaultConnection");
        }

        public int SaveHl7JsonToDatabase(string hl7Message)
        {
            try
            {
                // Convert HL7 to JSON
                string jsonMessage = ConvertHl7ToJson(hl7Message);
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Hl7Records (Hl7Json) OUTPUT INSERTED.ID VALUES (@Hl7Json);";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Hl7Json", jsonMessage);
                        int insertedId = Convert.ToInt32(cmd.ExecuteScalar());
                        conn.Close();
                        return insertedId;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving HL7 JSON: " + ex.Message);
                return -1;
            }
        }

        private string ConvertHl7ToJson(string hl7Message)
        {
            var hl7Segments = hl7Message.Split('\r');
            var hl7Dict = new Dictionary<string, Dictionary<int, string>>();
            foreach (var segment in hl7Segments)
            {
                var parts = segment.Split('|');
                if (parts.Length > 0)
                {
                    string segmentName = parts[0];
                    if (!hl7Dict.ContainsKey(segmentName))
                    {
                        hl7Dict[segmentName] = new Dictionary<int, string>();
                    }
                    for (int i = 1; i < parts.Length; i++)
                    {
                        hl7Dict[segmentName][i] = parts[i];
                    }
                }
            }
            return JsonConvert.SerializeObject(hl7Dict, Newtonsoft.Json.Formatting.Indented);
        }


        private void PublishToRabbitMQ(int id, string messageBody)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _rabbitMqOptions.HostName,
                Port = _rabbitMqOptions.Port,
                UserName = _rabbitMqOptions.UserName,
                Password = _rabbitMqOptions.Password
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _rabbitMqOptions.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.Headers = new Dictionary<string, object> { { "UniqueId", id } };
                var body = Encoding.UTF8.GetBytes(messageBody);
                channel.BasicPublish(exchange: "", routingKey: _rabbitMqOptions.QueueName, basicProperties: properties, body: body);
                Console.WriteLine($"✅ [RabbitMQ] Sent HL7 Event with ID: {id}");
            }
        }


    }
}
