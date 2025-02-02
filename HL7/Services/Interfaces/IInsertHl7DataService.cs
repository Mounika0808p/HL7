namespace HL7.Services.Interfaces
{
    public interface IInsertHl7DataService
    {
        int SaveHl7JsonToDatabase(string hl7Data);
    }
}
