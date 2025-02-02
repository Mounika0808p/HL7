using System.Globalization;
using HL7.Services.Interfaces;
using Microsoft.Data.SqlClient;

namespace HL7.Services
{
    public class InsertHl7DataService 
    {

        string _connectionString;
        IConfiguration _configuration;
        IConfigurationSection applicationSettings;
        public InsertHl7DataService(IConfiguration configuration) // Inject dependencies
        {
            _configuration = configuration;
            this.applicationSettings = configuration.GetSection("ConnectionStrings");
            _connectionString = applicationSettings.GetValue<string>("DefaultConnection");
        }
        public int InsertHl7Data(Dictionary<string, string> hl7Data)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {

                conn.Open();
                string query = "INSERT INTO Hl7Records (MessageHeader, PatientID, PatientName, Gender, DateOfBirth) " +
                               "VALUES (@MessageHeader, @PatientID, @PatientName, @Gender, @DateOfBirth); " +
                               "SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    string messageHeader = hl7Data.ContainsKey("MSH") ? hl7Data["MSH"] : string.Empty;
                    string[] pidSegments = hl7Data.ContainsKey("PID") ? hl7Data["PID"].Split('|') : new string[10];
                    string? patientId = pidSegments.Length > 2 ? pidSegments[1] : null;
                    string? patientName = pidSegments.Length > 4 ? pidSegments[5] : null;
                    string? gender = pidSegments.Length > 6 ? pidSegments[8] : null;
                    string? dobRaw = pidSegments.Length > 5 ? pidSegments[7] : null;

                    DateTime dateOfBirth;
                    bool isValidDate = DateTime.TryParseExact(dobRaw, "yyyyMMdd",
                                       CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfBirth);
                    cmd.Parameters.AddWithValue("@MessageHeader", messageHeader);
                    cmd.Parameters.AddWithValue("@PatientID", string.IsNullOrEmpty(patientId) ? (object)DBNull.Value : patientId);
                    cmd.Parameters.AddWithValue("@PatientName", string.IsNullOrEmpty(patientName) ? (object)DBNull.Value : patientName);
                    cmd.Parameters.AddWithValue("@Gender", string.IsNullOrEmpty(gender) ? (object)DBNull.Value : gender);
                    cmd.Parameters.AddWithValue("@DateOfBirth", isValidDate ? (object)dateOfBirth : DBNull.Value);
                    int insertedId = Convert.ToInt32(cmd.ExecuteScalar()); // ✅ Now it's safe to convert
                    conn.Close();
                    return insertedId;
                }
            }

        }
    }
}
