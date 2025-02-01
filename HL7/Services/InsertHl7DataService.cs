using HL7.Services.Interfaces;
using Microsoft.Data.SqlClient;

namespace HL7.Services
{
    public class InsertHl7DataService : IInsertHl7DataService
    {

        private readonly string _connectionString;
        IConfiguration _configuration;
        public InsertHl7DataService(IConfiguration configuration) 
        {
            this._configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection"); 
            
        }
        public int InsertHl7Data(Dictionary<string, string> hl7Data)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Hl7Records (MessageHeader, PatientID, PatientName, Gender, DateOfBirth) " + "OUTPUT INSERTED.Id VALUES (@MessageHeader, @PatientID, @PatientName, @Gender, @DateOfBirth)"; 
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MessageHeader", hl7Data["MSH"]);
                    cmd.Parameters.AddWithValue("@PatientID", hl7Data["PID"].Split('|')[2]);
                    cmd.Parameters.AddWithValue("@PatientName", hl7Data["PID"].Split('|')[4]);
                    cmd.Parameters.AddWithValue("@Gender", hl7Data["PID"].Split('|')[6]);
                    cmd.Parameters.AddWithValue("@DateOfBirth", hl7Data["PID"].Split('|')[5]);
                    return (int)cmd.ExecuteScalar(); // Returns the inserted record ID
                }
            }

        }
    }
}
